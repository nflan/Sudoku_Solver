using TMPro;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private string m_Value = null;
    [SerializeField] private TMP_Text m_ValueTxt = null;
    [SerializeField] private ValidateInput m_ValidateInput = null;

    void Start()
    {
        this.m_Value = "";
        if (!this.m_ValueTxt)
        {
            this.m_ValueTxt = GetComponentInChildren<TMP_Text>();
            if (!this.m_ValueTxt)
            {
                Debug.LogError("No Text field in " + this.gameObject.name + ".");
                Application.Quit();
            }
        }
        if (!this.m_ValidateInput)
        {
            this.m_ValidateInput = FindObjectOfType<ValidateInput>(true);
            if (!this.m_ValidateInput)
            {
                Debug.LogError("No ValidateInput found in " + this.gameObject.name + ".");
                Application.Quit();
            }
        }
    }

    public void WaitForInput()
    {
        this.m_ValidateInput.SetSquare(this);
        this.m_ValidateInput.gameObject.SetActive(true);
    }

    public void SetValue(string nb)
    {
        this.m_Value = nb;
        this.m_ValueTxt.text = m_Value;
    }
}
