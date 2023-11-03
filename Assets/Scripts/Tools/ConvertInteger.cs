using UnityEditor;
using UnityEngine;

namespace Tools
{
    public static class ConvertInteger 
    {   
        public static int ConverToOdd(int evenNum)
        {
            switch (evenNum % 2 ==0)
            {
                case true: return (evenNum + 1);
                case false: return evenNum;        
            }	
		}
    }
}