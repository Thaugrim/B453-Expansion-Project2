using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => (_instance ??= FindObjectOfType<UIManager>());

    [SerializeField] private GameObject _gravesParent, _heartsParent;
    [SerializeField] private Transform _bloodFill;
    [SerializeField] private Image _manaFill;

    private void Awake()
    {
        _instance = this;
    }

    public void InitializePlayerUI()
    {
        UpdateHearts();
        UpdateGraves();
        UpdateBlood();
        UpdateMana(100); // Added for spellcasting resource
    }
    public void InitializePlayerUI(PlayerController pc)
    {
        UpdateHearts(pc);
        UpdateGraves();
        UpdateBlood(pc);
        UpdateMana(100);
    }
    public void UpdateHearts()
    {
        for (int i = 0; i < _heartsParent.transform.childCount; i++)
        {
            GameObject gameObject = _heartsParent.transform.GetChild(i).gameObject;

            if (GameManager.Instance.PlayerController.Hp <= i)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }
    public void UpdateHearts(PlayerController pc)
    {
        for (int i = 0; i < _heartsParent.transform.childCount; i++)
        {
            GameObject gameObject = _heartsParent.transform.GetChild(i).gameObject;

            if (pc.Hp <= i)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }
    public void UpdateGraves()
    {
        for (int i = 0; i < _gravesParent.transform.childCount; i++)
        {
            GameObject gameObject = _gravesParent.transform.GetChild(i).gameObject;

            if (GameManager.Instance.Engraved.Count <= i)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }

    public void UpdateBlood()
    {
        float bloodFillHeightPerBloodPoint = (float)GameManager.Instance.PlayerController.Hp / 10; // 10 = maxBlood;

        _bloodFill.localScale = new Vector3(_bloodFill.localScale.x, bloodFillHeightPerBloodPoint, _bloodFill.localScale.z);
    }

    public void UpdateBlood(PlayerController pc)
    {
        float bloodFillHeightPerBloodPoint = (float)pc.Hp / 10; // 10 = maxBlood;

        _bloodFill.localScale = new Vector3(_bloodFill.localScale.x, bloodFillHeightPerBloodPoint, _bloodFill.localScale.z);
    }

    /* 
     * 
     *   ADDED FUNCTIONS FOR UI UPDATE
     *  
     */

    public void UpdateMana(float amount)  // Added for spellcast resource
    {
        float fill = amount / 100;

        _manaFill.fillAmount = fill;
    }
}
