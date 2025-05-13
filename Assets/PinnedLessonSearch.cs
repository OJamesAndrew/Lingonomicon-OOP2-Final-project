using UnityEngine;
using TMPro;  
using System.Collections.Generic;

public class PinnedLessonSearch : MonoBehaviour
{
    public TMP_InputField searchField; 
    public Transform lessonsParent; 

    private List<GameObject> lessonItems = new List<GameObject>();

    private void Start()
    {
        foreach (Transform child in lessonsParent)
        {
            lessonItems.Add(child.gameObject);
        }

        searchField.onValueChanged.AddListener(delegate { SearchLessons(); 
            
            foreach (Transform child in lessonsParent)
            {
                lessonItems.Add(child.gameObject);
            }
        });
    }

    public void SearchLessons()
    {
        string searchText = searchField.text.ToLower(); 

        foreach (GameObject lesson in lessonItems)
        {
            TMP_Text lessonText = lesson.GetComponentInChildren<TMP_Text>(); 
            if (lessonText != null)
            {

                if (lessonText.text.ToLower().Contains(searchText) && !lessonText.text.ToLower().Contains("{lang}: {question}"))
                {
                    lesson.SetActive(true);
                }
                else
                {
                    lesson.SetActive(false);
                }
                
            }
        }
    }

    public void ClearSearchBar()
    {
        searchField.text = string.Empty;
    }
}
