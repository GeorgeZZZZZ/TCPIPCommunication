using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace Georgescript
{
    public static class TCPProtocol
    {
        // example : [head][num]2;[c]01;[c]02;[let]3;[c]abc;[CR][LF]
        // protocol header
        private static readonly byte[] Head_Bytes = { 60, 104, 101, 97, 100, 62 };   //<head>
        public static readonly string Head_Str = Encoding.Default.GetString(Head_Bytes);

        // content types
        private static readonly byte[] num_Bytes = { 40, 110, 117, 109, 41 };   //(num), read follow content type as number and to control max bytes to read, this type has max number of 3
        public static readonly string num_Str = Encoding.Default.GetString(num_Bytes);

        private static readonly byte[] letter_Bytes = { 40, 108, 101, 116, 41 };   //(let), read follow content type as letter and to control max bytes to read, this type has max number of 3
        public static readonly string letter_Str = Encoding.Default.GetString(letter_Bytes);

        private static readonly byte[] content_Bytes = { 40, 99, 41 };   //(c), content
        public static readonly string content_Str = Encoding.Default.GetString(content_Bytes);

        private static readonly byte separater_Byte = 59;   //;, end of each content type
        public static readonly string separater_Str = Convert.ToChar(separater_Byte).ToString();


        // error
        private static readonly byte[] notmutch_Bytes = { 91, 80, 114, 111, 116, 111, 99, 111, 108, 78, 111, 116, 77, 97, 116, 99, 104, 93 };   //[ProtocolNotMatch]
        public static readonly string notmutch_Str = Encoding.Default.GetString(notmutch_Bytes);

        // done
        private static readonly byte[] done_Bytes = { 91, 68, 111, 110, 101, 93 };   //[Done]
        public static readonly string done_Str = Encoding.Default.GetString(done_Bytes);

        // ending
        private static readonly byte[] End_Bytes = { 60, 13, 62, 60, 10, 62 };    //<CR><LF>
        public static readonly string End_Str = Encoding.Default.GetString(End_Bytes);

        // default max read length
        public static readonly int defaultNumberMaxLength = 5;
        public static readonly int defaultLetterMaxLength = 5;
        public static readonly int defaultMaxLengthSetLength = 3;
    }
}