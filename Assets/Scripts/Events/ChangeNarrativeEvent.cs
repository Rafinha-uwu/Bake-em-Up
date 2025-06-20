using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNarrativeEvent : MonoBehaviour
{
    public delegate void NarrativeHandler(string narrator);
    public static event NarrativeHandler OnChange;

    public static void ChangeNarrator(string narrator)
    {
        OnChange?.Invoke(narrator);
    }
}