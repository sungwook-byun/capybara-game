using UnityEngine;

public class OxygenZone : MonoBehaviour
{
    // 산소 회복 처리
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerCollision pc = other.GetComponentInParent<PlayerCollision>();
        if (pc != null)
            pc.RechargeOxygen(Time.deltaTime * pc.oxygenRechargeRate);
    }
}
