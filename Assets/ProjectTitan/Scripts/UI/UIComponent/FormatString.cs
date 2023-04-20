using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Titan
{
    [RequireComponent(typeof(TMP_Text))]
    public class FormatString : MonoBehaviour
    {
        private TMP_Text _target;
        private string _formatStr;

        public void Format(params object[] args)
        {
            _target.text = string.Format(_formatStr, args);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        private void OnValidate()
        {
            _target = GetComponent<TMP_Text>();
            _formatStr = _target.text;
        }
#endif        
    }
}
