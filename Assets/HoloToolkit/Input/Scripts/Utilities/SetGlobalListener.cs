// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private void OnEnable()
        {

            //InputManager.Instance.AddGlobalListener(gameObject);エラー出たため変更

            if (InputManager.Instance != null)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
            else
            {
                //Debug.LogWarning("SetGlobalListener used but no InputManager found in the scene. This object will not be registered, even if an InputManager is added.");
            }
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }
    }
}
