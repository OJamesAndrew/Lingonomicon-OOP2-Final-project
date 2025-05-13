using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ChartDataRetriever : MonoBehaviour
{
    private string phpURL = "http://localhost/lingonomicon/ReadData.php";
    
    public IEnumerator RetrieveChartValues(string query, System.Action<float[]> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("sqlPost", query);

        using (UnityWebRequest www = UnityWebRequest.Post(phpURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error retrieving data: " + www.error);
                callback(null);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                Debug.Log("Received JSON: " + jsonResult);

                try
                {
                    string cleanedJson = jsonResult.Replace(" ", ""); 

                    string trimmed = cleanedJson.Trim('[', ']'); 

                    float[] retValues;

                    if (trimmed.Contains("},{")) 
                    {
                        
                        string[] objectStrings = trimmed.Split(new string[] { "},{" }, System.StringSplitOptions.RemoveEmptyEntries);

                        retValues = new float[objectStrings.Length];

                        for (int i = 0; i < objectStrings.Length; i++)
                        {
                            string cleanObject = objectStrings[i].Replace("{", "").Replace("}", "").Replace("\"", "");
                            string[] keyValue = cleanObject.Split(':');

                            if (keyValue.Length == 2)
                            {
                                retValues[i] = float.Parse(keyValue[1]);
                            }
                            else
                            {
                                Debug.LogError($"Unexpected format at key-value {i}: {objectStrings[i]}");
                                break; 
                            }
                        }
                    }
                    else 
                    {
                        trimmed = trimmed.Trim('{', '}'); 
                        trimmed = trimmed.Replace("\"", ""); 

                        string[] keyValues = trimmed.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                        retValues = new float[keyValues.Length];

                        for (int i = 0; i < keyValues.Length; i++)
                        {
                            string[] keyValue = keyValues[i].Split(':');

                            if (keyValue.Length == 2)
                            {
                                retValues[i] = float.Parse(keyValue[1]);
                            }
                            else
                            {
                                Debug.LogError($"Unexpected format at key-value {i}: {keyValues[i]}");
                                break;
                            }
                        }
                    }

                    callback(retValues);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Database empty OR JSON Parsing error: " + ex.Message);
                    callback(null);
                }

            }
        }
    }


}
