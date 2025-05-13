using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "qtsData", menuName = "Scriptable Objects/qtsData")]
public class qtsData : ScriptableObject
{
    [System.Serializable]
    public struct Question
    {
        public string questionText;
        public string[] choices;
        public int correctChoiceIndex;

    }
    public Question[] questions;

    
}
