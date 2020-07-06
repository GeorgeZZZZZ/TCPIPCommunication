using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Georgescript
{
    public class TCPUnpackage
    {
        private static int headLabelLength = TCPProtocol.Head_Str.Length;
        private static int endLabelLength = TCPProtocol.End_Str.Length;
        private static int contentLabelLength = TCPProtocol.content_Str.Length;
        private int numberMaxLength = TCPProtocol.defaultNumberMaxLength;
        private int letterMaxLength = TCPProtocol.defaultLetterMaxLength;
        private static readonly int lengthSetLength = TCPProtocol.defaultMaxLengthSetLength;

        public bool MessageIn(string _received, out List<float> _contentNUM, out List<string> _contentSTR)
        {
            _contentSTR = new List<string>();
            _contentNUM = new List<float>();
            bool isDone = Process(_received, out _contentNUM, out _contentSTR);
            if (!isDone)
            {
                _contentSTR.Clear();
                _contentNUM.Clear();
            }
            return isDone;
        }

        // if unpackage done then return true
        private bool Process(string _msg, out List<float> _contontNum, out List<string> _contontStr)
        {
            const bool _done = true;
            const bool _err = false;
            _contontNum = new List<float>();
            _contontStr = new List<string>();

            int msgLength = _msg.Length;
            if (msgLength < 10) return _err;  // if no message then return error

            try
            {
                // check header
                string head = _msg.Substring(0, headLabelLength);
                if (head != TCPProtocol.Head_Str) return _err;

                // start compare content
                int? labelStartIndex = null;
                int? labelStopIndex = null;
                int? separateIndex = null;
                bool? isNUM = null;
                for (int i = headLabelLength; i < msgLength - endLabelLength; i++)
                {
                    if (_msg[i] == TCPProtocol.content_Str[0]) labelStartIndex = i;
                    if (_msg[i] == TCPProtocol.content_Str[contentLabelLength - 1]) labelStopIndex = i;
                    if (_msg[i] == TCPProtocol.separater_Str[0])
                    {
                        separateIndex = i;
                        // if not read any of index symble then error
                        if (labelStartIndex == null || labelStopIndex == null || separateIndex == null) return _err;
                        if (labelStartIndex >= labelStopIndex) return _err;
                        if (separateIndex < labelStopIndex) return _err;
                        string label = _msg.Substring(labelStartIndex.Value, (labelStopIndex - labelStartIndex).Value + 1);
                        string content = _msg.Substring(labelStopIndex.Value + 1, (separateIndex - labelStopIndex).Value - 1);
                        if (label == TCPProtocol.num_Str)
                        {   // next content is number
                            // content length safe check
                            if (content.Length > lengthSetLength) return _err;
                            if (float.TryParse(content, out float contentOut)) numberMaxLength = Convert.ToInt32(contentOut);
                            else return _err;
                            isNUM = true;
                        }
                        else if (label == TCPProtocol.letter_Str)
                        {   // next content is letter
                            // content length safe check
                            if (content.Length > lengthSetLength) return _err;
                            if (float.TryParse(content, out float contentOut)) letterMaxLength = Convert.ToInt32(contentOut);
                            else return _err;
                            isNUM = false;
                        }
                        else if (label == TCPProtocol.content_Str)
                        {
                            if (isNUM == true)
                            {
                                // max number length safe check
                                if (content.Length > numberMaxLength) return _err;
                                if (float.TryParse(content, out float contentOut)) _contontNum.Add(contentOut);
                                else return _err;
                            }
                            else if (isNUM == false)
                            {
                                // max letter length safe check
                                if (content.Length > letterMaxLength) return _err;
                                _contontStr.Add(content);
                            }
                            else return _err;
                        }
                        else return _err;

                        labelStartIndex = labelStopIndex = separateIndex = null;   // reset index cache for safe check
                    }
                }

                // check ending
                if (_msg.Substring(msgLength - endLabelLength) != TCPProtocol.End_Str) return _err;

                // corrently if no content or content protocol wrong there is no error check
            }
            catch
            {
                return _err;
            }

            return _done;

        }
    }

}