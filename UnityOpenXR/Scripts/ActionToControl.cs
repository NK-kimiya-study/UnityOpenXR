using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace UnityVR
{
    public class ActionToControl : MonoBehaviour
    {
        //TextMeshProUGUI型のフィールドを定義
        //[SerializeField]->（private または protected）フィールドでも、Unityエディタのインスペクタからそのフィールドにアクセスして値を設定できる
        [SerializeField] protected TextMeshProUGUI displayMessage;
        //InputActionReference型のフィールドを定義
        [SerializeField] InputActionReference actionReference;
        //このクラスが準備完了かどうか
        protected bool isReady = true;
        //エラーメッセージを格納
        protected string errorMessage;
        //特定の入力アクションを格納
        InputAction action;

        void Awake()
        {
            if(displayMessage is null){
                //アプリケーションを閉じる
                Application.Quit();
            }
            if(actionReference is null || (action = actionReference.action) is null)
            {
                //どの部分に問題があるかを特定
                isReady = false;
                errorMessage = "#actionReference";
            }
        }

        void OnEnable()
        {
            if(!isReady){return;}

            //actionが開始されるとOnActionStartedメソッドが呼び出される
            action.started += OnActionStarted;
            //actionが実行されるとOnActionPerformedメソッドが呼び出される
            action.performed += OnActionPerformed;
            //actionがキャンセルされるとOnActionCanceledメソッドが呼び出される
            action.canceled += OnActionCanceled;
            //actionを有効にする
            action.Enable();
        }

        //オブジェクトが無効になったとき
        void OnDisable()
        {
            if(!isReady){return;}
            //入力アクションに関するイベントハンドラを解除してアクションを無効にする
            action.Disable();
            action.started -= OnActionStarted;
            action.performed -= OnActionPerformed;
            action.canceled -= OnActionCanceled;
        }

        //入力アクションが特定のイベント（開始、実行、キャンセル）を受け取ったときに呼び出されるメソッドを定義
        protected virtual void OnActionStarted(InputAction.CallbackContext ctx){}

        protected virtual void OnActionPerformed(InputAction.CallbackContext ctx){}

        protected virtual void OnActionCanceled(InputAction.CallbackContext ctx){}

    }

    
}

