using TMPro;
using UnityEngine;

public class Square : MonoBehaviour
{
    [Header("Square information")]
    [SerializeField] private string m_Value = null;
    [SerializeField] private TMP_Text m_ValueTxt = null;
    [SerializeField] private ValidateInput m_ValidateInput = null;
    
    [SerializeField] private int m_Index = 0;

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

    public void SetIndex(int index)
    {
        this.m_Index = index;
    }

    public int GetIndex()
    {
        return this.m_Index;
    }

    public string GetValue()
    {
        return this.m_Value;
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
