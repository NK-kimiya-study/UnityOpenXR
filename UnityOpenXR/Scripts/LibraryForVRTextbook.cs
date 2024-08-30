using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;

namespace UnityVR
{
    //公開クラスを定義
    public class LibraryForVRTextbook
    {
        //呼び出し元のソースファイルパスを受け取り、そのファイル名を返す
        public static string GetSourceFileName([CallerFilePath] string sourceFilePath = "")
        =>Path.GetFileName(sourceFilePath.Replace(@"\","/"));

        //GetCallerMember関数を呼び出しているメソッド名などを返す
        public static string GetCallerMember([CallerMemberName] string memberName = "")
        =>memberName;
    }
}
