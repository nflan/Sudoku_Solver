using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Square : MonoBehaviour
{
    [Header("Square informations")]
    [SerializeField] private string m_Value = "";
    [SerializeField] private TMP_Text m_ValueTxt = null;
    [SerializeField] private ValidateInput m_ValidateInput = null;
    [SerializeField] private bool m_IsManuallySet = false;
    [SerializeField] private Dictionary<string, bool> m_Tried;
    
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
        this.ResetTried();
    }

    public void ResetTried()
    {
        m_Tried = new Dictionary<string, bool>
        {
            {"1", false},
            {"2", false},
            {"3", false},
            {"4", false},
            {"5", false},
            {"6", false},
            {"7", false},
            {"8", false},
            {"9", false},
        };
    }

    public bool CheckTried(int value)
    {
        return m_Tried[value.ToString()];
    }

    public void ResetSquare()
    {
        this.m_Value = "";
        this.m_ValueTxt.text = "";
        this.m_IsManuallySet = false;
        this.ResetTried();
    }

    public void SetIsManuallySet(bool isManuallySet)
    {
        this.m_IsManuallySet = isManuallySet;
    }

    public bool GetIsManuallySet()
    {
        return this.m_IsManuallySet;
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
        this.m_Tried[nb] = true;
    }
}
