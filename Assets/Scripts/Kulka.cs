using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kulka : MonoBehaviour{
    //--------------------------------------------
    // Ball radius and mass
    public float radius = 1;
    public float mass = 1;
    // From how many balls this ball is created
    private int howManyBalls = 0;
    // How strong gravity zone works
    public float gravityZonePower;
    // Drag value (Air resistance)
    public float dragValue;
    // Explode max force
    public float explodePower;
    
    //--------------------------------------------
    // Is collision turned on
    public bool collisionTurnedOn = false;
    // Time till turning collision back on
    private float calcToTurnBack = 0.5f;
    
    //--------------------------------------------
    // Reference to Rigidbody
    private Rigidbody rigidBody;
    // Array of SphereColliders
    private SphereCollider[] colliders;
    // Ball prefab
    public GameObject spherePrefab;
    
    //--------------------------------------------
    // Resize sphere to new radius
    void ResizeSphere(){
        transform.localScale = new Vector3(radius, radius, radius);
    }
    
    //--------------------------------------------
    // When colliding with something
    void OnCollisionEnter(Collision collision){
        // If there's less balls than max balls
        if(Controller.spheresAmount < Controller.MAX_BALLS){
            // Save point of contact to var
            ContactPoint contact = collision.contacts[0];   
            // Save reference to collider's script
            Kulka script = collision.collider.GetComponent<Kulka>();
            // If script exists, either check if this object mass is bigger or if equal, check if this object is older
            if(script != null && (script.mass < mass) || (script.mass == mass && script.GetInstanceID() > GetInstanceID())){
                // Get radius and mass of collider
                float otherRadius = script.radius;
                float otherMass = script.mass;
                // Add +1 to "how many balls this ball is created from"
                howManyBalls++;
                // Destroy collider
                Destroy(collision.collider.gameObject);
                // Add mass of collider to object
                rigidBody.mass += otherMass;
                // Set mass variable to updated value
                mass = rigidBody.mass;
                // Set radius to sqrt of mass
                radius = Mathf.Sqrt(mass);
                // Resize ball
                ResizeSphere();
            }
        }
    }
    
    //--------------------------------------------
    // When in gravity zone
    void OnTriggerStay(Collider other){
        // If collider has rigidbody and has collision turned on
        if(other.attachedRigidbody && collisionTurnedOn){
            // Get difference vector between balls
            Vector3 calcTransform = new Vector3(transform.position.x-other.transform.position.x, 
            transform.position.y-other.transform.position.y, 0);
            // Normalize that vector
            calcTransform.Normalize();
            // Check if there's more balls than max balls
            if(Controller.spheresAmount >= Controller.MAX_BALLS){
                // Reverse vector
                calcTransform = -calcTransform;
            }
            // Apply new force to collider
            other.attachedRigidbody.AddForce(calcTransform*mass*gravityZonePower);
        }
    }
    
    //--------------------------------------------
    // When ball is created
    void Start(){
        // Update balls count
        Controller.UpdateBalls();
        // Load ball prefab
        spherePrefab = (GameObject)Resources.Load("Sphere");
        // Get references to Rigidbody and SphereCollider
        rigidBody = GetComponent<Rigidbody>();
        colliders = GetComponents<SphereCollider>();
        // Turn on/off collisions
        colliders[0].enabled = collisionTurnedOn;
        colliders[1].enabled = collisionTurnedOn;
    }    
    
    //--------------------------------------------
    // When ball is destroyed
    void OnDestroy(){
        // Update balls count
        Controller.UpdateBalls();
    }
    
    //--------------------------------------------
    // Every fixed frame
    void FixedUpdate(){
        // Apply drag
        rigidBody.velocity /= dragValue;
        // If collisions are off
        if(!collisionTurnedOn){
            // Decrease timer
            calcToTurnBack -= Time.deltaTime;
            // If timer is smaller or equal 0
            if(calcToTurnBack <= 0.0f){
                // Turn collisions on
                collisionTurnedOn = true;
                colliders[0].enabled = collisionTurnedOn;
                colliders[1].enabled = collisionTurnedOn;
            }
        }
        // If mass is bigger or equal 50 and there's less balls than max
        if(mass >= 50 && Controller.spheresAmount < Controller.MAX_BALLS){
            // Turn collisions off
            colliders[0].enabled = false;
            colliders[1].enabled = false;
            // Create balls from amount of balls that created this ball
            for(int i=0; i<howManyBalls; i++){
                // If there's less balls than max
                if(Controller.spheresAmount < Controller.MAX_BALLS){
                    // Create new ball at this ball position
                    GameObject obj = Instantiate(spherePrefab, transform.position, new Quaternion(0, 0, 0, 0));
                    // Apply force of explosion
                    obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-explodePower, explodePower), Random.Range(-explodePower, explodePower), 0.0f));
                    // Update balls count
                    Controller.UpdateBalls();
                } else {
                    break;
                }
            }
            // Destroy self
            Destroy(gameObject);
        }
    }
}
