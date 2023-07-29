using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // Function to be called when the exit button is clicked
    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
