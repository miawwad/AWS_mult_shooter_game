using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [SerializeField] private Transform debugTransform;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;

    private Animator animator;

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }
    
    // // Update is called once per frame
   private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        
         //math to get center of screen
        Vector2 ScreenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //to get our mouse and cross hair matching.
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenterPoint);
        //SECOND AAIMING MECHANISUM 
        // Transform hitTransform = null; //for our hit scan
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)){
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            // hitTransform = raycastHit.transform;
        }
        
        if(starterAssetsInputs.aim){
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            //aim animation!
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }else{
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f)); //aim 
        }

        if(starterAssetsInputs.shoot){
            //SECOND AAIMING MECHANISUM 
            // if(hitTransform != null){
            //     //same logic as on bullets
            //     //add hit/no hit identifier
            //     if(hitTransform.GetComponent<BulletTarget>() != null) {
            //         //Hit target
            //         Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            //     }else {
            //         //hit smth else
            //         Instantiate(vfxHitRed, transform.position, Quaternion.identity);
            //     }
            // }
            //UNCOMMENT ABOVE & COMMENT FIRST 2 BELOW TO TRY OTHER SHOOTING
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }

    }
}
