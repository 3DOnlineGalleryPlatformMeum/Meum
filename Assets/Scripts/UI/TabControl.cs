﻿using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    /*
     * @brief 환경설정에서 탭을 관리(버튼클릭시 전환 담당)
     */
    public class TabControl : MonoBehaviour
    {
        [SerializeField] private bool autoDisable;
        [SerializeField] private Button[] buttons;
        [SerializeField] private TextMeshProUGUI[] buttonTexts;
        [SerializeField] private GameObject[] tabs;

        [Header("Button Color")] 
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color activatedTextColor;
        [SerializeField] private Color deactivatedColor;
        [SerializeField] private Color deactivatedTextColor;
        

        private void Awake()
        {
            Assert.AreEqual(buttons.Length, buttonTexts.Length);
            Assert.AreEqual(buttons.Length, tabs.Length);
            
            if(autoDisable)
                gameObject.SetActive(false);
            
            for (var i = 0; i < buttons.Length; ++i)
            {
                var temp = i;
                buttons[i].onClick.AddListener(() => ChangeTab(temp));
            }
            
            ChangeTab(0);
        }

        public void ChangeTab(int idx)
        {
            for (var i = 0; i < tabs.Length; ++i)
            {
                Assert.IsNotNull(tabs[i]);
                Assert.IsNotNull(buttons[i]);
                tabs[i].SetActive(false);
                buttons[i].image.color = deactivatedColor;
                if(!ReferenceEquals(buttonTexts[i], null))
                    buttonTexts[i].color = deactivatedTextColor;
            }
            
            tabs[idx].SetActive(true);
            buttons[idx].image.color = activatedColor;
            if(!ReferenceEquals(buttonTexts[idx], null))
                buttonTexts[idx].color = activatedTextColor;
        }
    
        public void Show() {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}