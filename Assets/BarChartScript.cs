using UnityEngine;
using TMPro;

public class BarChartManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] bars; 
    [SerializeField] private TMP_Text[] barLabels; 
    [SerializeField] private float maxBarHeight = 200f; 
    [SerializeField] GameObject AnalyticsCont;
    [SerializeField] GameObject EmptyMSG;

    private void Start()
    {
        AnalyticsCont.SetActive(true);
        EmptyMSG.SetActive(false);
        ShowAverageScore();
    }
    public void UpdateBarChart(float[] values, string[] labels)
    {
        if (values.Length != bars.Length || labels.Length != bars.Length)
        {
            if(values.Length < bars.Length || values == null)
            {
                AnalyticsCont.SetActive(false);
                EmptyMSG.SetActive(true);
            }
            else
            {
                Debug.LogError("Mismatch between data and bar objects!");
                return;
            }
        }

        int maxValue = (int)Mathf.Max(values);

        for (int i = 0; i < bars.Length; i++)
        {
            float heightPercent = (float)values[i] / maxValue;
            bars[i].sizeDelta = new Vector2(bars[i].sizeDelta.x, heightPercent * maxBarHeight);
            barLabels[i].text = labels[i] + "\n" + values[i].ToString();
        }
    }

    public ChartDataRetriever dataRetriever; 

    public void ShowAverageScore()
    {
        string query = "SELECT AVG(score) FROM leaderboards GROUP BY lang;";
        StartCoroutine(dataRetriever.RetrieveChartValues(query, (float[] values) =>
        {
            if (values != null)
            {
                string[] labels = { "Spanish", "Portuguese", "Japanese" }; 
                UpdateBarChart(values, labels);
            }
        }));
    }

    public void ShowScoreDistribution()
    {
        string query = @"SELECT 
                        SUM(CASE WHEN score <= 50 THEN 1 ELSE 0 END),
                        SUM(CASE WHEN score > 50 AND score <= 100 THEN 1 ELSE 0 END),
                        SUM(CASE WHEN score > 100 THEN 1 ELSE 0 END)
                     FROM leaderboards;";
        StartCoroutine(dataRetriever.RetrieveChartValues(query, (float[] values) =>
        {
            if (values != null)
            {
                string[] labels = { "0-50", "51-100", "101+" };
                UpdateBarChart(values, labels);
            }
        }));
    }

    public void ShowPopularLanguage()
    {
        string query = "SELECT COUNT(*) FROM leaderboards GROUP BY lang ORDER BY lang DESC;";
        StartCoroutine(dataRetriever.RetrieveChartValues(query, (float[] values) =>
        {
            if (values != null)
            {
                string[] labels = { "Spanish", "Portuguese", "Japanese" }; 
                UpdateBarChart(values, labels);
            }
        }));
    }
}
