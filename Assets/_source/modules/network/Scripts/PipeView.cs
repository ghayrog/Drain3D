using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

namespace Network
{
    internal class PipeView : MonoBehaviour
    {
        [SerializeField]
        private int _startI;

        [SerializeField]
        private int _startJ;

        [SerializeField]
        private int _endI;

        [SerializeField]
        private int _endJ;

        [ProPlayButton]
        internal void RedrawPipe()
        {
            float startX = NetworkView.CellIndexToCoord(_startI);
            float startY = NetworkView.CellIndexToCoord(_startJ);
            float endX = NetworkView.CellIndexToCoord(_endI);
            float endY = NetworkView.CellIndexToCoord(_endJ);
            float eulerAngle = Mathf.Atan2(endX-startX, endY - startY) / Mathf.PI * 180;
            float middleX = (startX + endX) / 2;
            float middleY = (startY + endY) / 2;
            float scale = Mathf.Sqrt((_startI - _endI) * (_startI - _endI) + (_startJ - _endJ) * (_startJ - _endJ));
            transform.position = new Vector3(middleX, transform.position.y, middleY);
            transform.rotation = Quaternion.Euler(0, eulerAngle, 0);
            transform.localScale = new Vector3(1, 1, scale);
            //Debug.Log($"Start: ({startX}, {startY}), End: ({endX}, {endY}), Angle cos: {(endX - startX) / (endY - startY)}, Scale: {scale}");
        }
    }
}
