using System;
using System.Linq;
using UnityEngine;

namespace AllieJoe.SudokuSolver
{
    public  class SudokuHelper : MonoBehaviour
    {
        public static SudokuHelper Instance;

        public bool useOrder;
        public string orderToFollow;
        private int[] order;
        private int index = 0;
        
        private string orderLog;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            order = orderToFollow.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
        }

        private void OnDestroy()
        {
            Debug.LogError(orderLog);
        }

        public  void RegisterCell(int i)
        {
            orderLog += $"{i},";
        }

        public int NextCell()
        {
            int i = order[index];
            index++;
            return i;
        }

        public bool HasNextCell() => index < order.Length;

    }
}