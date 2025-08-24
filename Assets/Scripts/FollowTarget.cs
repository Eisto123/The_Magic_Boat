using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FollowTarget : MonoBehaviour
{
   //gengsui
    public Transform target;          
    public Vector3 offset;
    public bool smooth = true;
    public float smoothTime = 0.08f;

   
    public bool stopWhenTargetLost = true;   
    public bool autoDestroyWhenDone = true;  

    private Vector3 velocity;
    private ParticleSystem ps;
    private bool hasStoppedForLostTarget = false;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        
        if (target == null)
        {
            if (stopWhenTargetLost && !hasStoppedForLostTarget)
            {
                
                var main = ps.main;
                main.loop = false;
                ps.Stop();
                ps.Play();

                
                float killTime = main.duration + main.startLifetime.constantMax + 0.1f;
                if (autoDestroyWhenDone)
                    Destroy(gameObject, killTime);

                hasStoppedForLostTarget = true;
            }

            
            if (autoDestroyWhenDone && hasStoppedForLostTarget && !ps.IsAlive(true))
                Destroy(gameObject);

            return;
        }

        
        Vector3 goal = target.position + offset;
        if (smooth)
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
        else
            transform.position = goal;
    }
}
