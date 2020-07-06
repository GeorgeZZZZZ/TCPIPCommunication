using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Georgescript
{
    public class TCPPackage
    {
        private List<string> allStringContentValue = new List<string>();
        private List<float> allNumberContentValue = new List<float>();
        private int maxNumLength = TCPProtocol.defaultNumberMaxLength;
        private int maxLetterLength = TCPProtocol.defaultLetterMaxLength;
        private static readonly int lengthSetLength = TCPProtocol.defaultMaxLengthSetLength;

        public void ChangeContentMaxLength(int _num, int _letter)
        {
            if (_num.ToString().Length > lengthSetLength || _letter.ToString().Length > lengthSetLength) Debug.LogError("Max content number given too large");
            else
            {
                maxNumLength = _num;
                maxLetterLength = _letter;
            }
        }
        public void ContentAddString(string _contentsIn)
        {
            if (_contentsIn.Length > maxLetterLength) Debug.LogError("TCP letter content input length not match");
            else allStringContentValue.Add(_contentsIn);
        }
        public void ContentAddNumber(float _contentsIn)
        {
            if (_contentsIn.ToString().Length > maxNumLength) Debug.LogError("TCP number content input length not match");
            else allNumberContentValue.Add(_contentsIn);
        }
        public void ContenctClearAll()
        {
            allStringContentValue.Clear();
            allNumberContentValue.Clear();
        }

        public string GetPackagedMessage()
        {
            string reSTR = null;
            reSTR = FinalOrganize();
            ContenctClearAll();
            return reSTR;
        }

        public string GetDoneMessage()
        {
            return TCPProtocol.Head_Str + TCPProtocol.done_Str + TCPProtocol.End_Str;
        }
        public string GetNotMatchMessage()
        {
            return TCPProtocol.Head_Str + TCPProtocol.notmutch_Str + TCPProtocol.End_Str;
        }

        private string OrganizeStringList()
        {
            string midSTR = null;
            foreach (string c in allStringContentValue) midSTR += TCPProtocol.content_Str + c + TCPProtocol.separater_Str;
            return TCPProtocol.letter_Str + maxLetterLength + TCPProtocol.separater_Str + midSTR;
        }
        private string OrganizeNumberList()
        {
            string midSTR = null;
            foreach (float c in allNumberContentValue) midSTR += TCPProtocol.content_Str + c.ToString() + TCPProtocol.separater_Str;
            return TCPProtocol.num_Str + maxNumLength + TCPProtocol.separater_Str + midSTR;
        }
        private string FinalOrganize()
        {
            string midSTR = null;
            if (allNumberContentValue.Count > 0) midSTR += OrganizeNumberList();
            if (allStringContentValue.Count > 0) midSTR += OrganizeStringList();
            return TCPProtocol.Head_Str + midSTR + TCPProtocol.End_Str;
        }
    } 
}
