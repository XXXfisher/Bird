using UnityEngine;

public class TrashZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Recycle"))
        {
            // 进入垃圾桶的物体如果是可回收物，直接销毁
            Destroy(other.gameObject);
        }
    }
}
