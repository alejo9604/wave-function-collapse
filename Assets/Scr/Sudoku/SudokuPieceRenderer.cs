using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AllieJoe.SudokuSolver.View
{
    public class SudokuPieceRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _domainLabelPanel;
        [SerializeField] private TextMeshProUGUI _collapsedText;

        [Space(20)] [SerializeField] private GameObject[] borders;
        
        private TextMeshProUGUI[] _domainTexts;

        public void Init()
        {
            _domainTexts = _domainLabelPanel.GetComponentsInChildren<TextMeshProUGUI>();
        }

        public void SetBorders(int x, int y, int quadrantSize)
        {
            if(y + 1< quadrantSize * quadrantSize)
                borders[0].SetActive((y + 1) % quadrantSize == 0);
            if(x + 1 < quadrantSize * quadrantSize)
                borders[1].SetActive((x + 1) % quadrantSize == 0);
        }

        public void SetData(int value, List<int> domain)
        {
            _collapsedText.color = Color.white;
            _collapsedText.text = value > 0 ? value.ToString() : "";
            for (int i = 0; i < _domainTexts.Length; i++)
            {
                int domainValue = i + 1;
                if (domain.Contains(domainValue))
                    _domainTexts[i].text = domainValue.ToString();
                else
                    _domainTexts[i].text = "";
            }

            if (value <= 0 && domain.Count == 0)
            {
                _collapsedText.text = "X";
                Highlight();
            }
        }

        public void Highlight()
        {
            _collapsedText.color = Color.yellow;
        }

        public void HighlightCompleted()
        {
            _collapsedText.color = Color.green;
        }
        
        public void HighlightNoSolution()
        {
            _collapsedText.color = Color.red;
        }
    }
}