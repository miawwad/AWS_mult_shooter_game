using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private Rigidbody bulletRigidbody;

    private void Awake(){
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    //now our bullet will move forward once it respawns.
    private void Start(){
        float speed = 40f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    //for our collision testing
    private void OnTriggerEnter(Collider other){
        //add hit/no hit identifier
        if(other.GetComponent<BulletTarget>() != null) {
            //Hit target
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }else {
            //hit smth else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
