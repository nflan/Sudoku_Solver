using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorTypo : MonoBehaviour
{
    [Header("ValidateInput Information")]
    [SerializeField] private Image m_ErrorPanel = null;
    [SerializeField] private TMP_Text m_ErrorTxt = null;

    void Awake()
    {
        if (!m_ErrorPanel)
        {
            m_ErrorPanel = this.GetComponentInChildren<Image>(true);
            if (!m_ErrorPanel)
            {
                Debug.LogError("No Error Panel in the scene.");
                Application.Quit();
            }
            m_ErrorPanel.gameObject.SetActive(false);
        }
        if (!m_ErrorTxt)
        {
            m_ErrorTxt = this.GetComponentInChildren<TMP_Text>(true);
            if (!m_ErrorTxt)
            {
                Debug.LogError("No Error Text in the scene.");
                Application.Quit();
            }
            m_ErrorTxt.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine(DisableErrorPanel());
    }

    public void SetErrorTxt(string error)
    {
        this.m_ErrorTxt.text = error;
    }

    private IEnumerator DisableErrorPanel()
    {
        yield return new WaitForSeconds(2);
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
}
