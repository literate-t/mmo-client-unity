using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    Transform _bar = null;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        _bar.localScale = new Vector3(ratio, 1, 1);
    }
}
