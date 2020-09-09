using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour{
    //--------------------------------------------
    // Time between new balls
    private const float NEXT_BALL_TIME = 0.25f;
    // Time till next ball spawn
    private float timeTillNext = NEXT_BALL_TIME;
    
    //--------------------------------------------
    // Max balls possible
    public const int MAX_BALLS = 250;
    // Current ball count
    public static int spheresAmount = 0;
    
    //--------------------------------------------
    // Reference to text counter
    public UnityEngine.UI.Text counter;
    // Ball prefab
    public GameObject spherePrefab;
    
    //--------------------------------------------
    // Update balls count
    static public void UpdateBalls(){
        var gos = GameObject.FindGameObjectsWithTag("Ball");
        spheresAmount = gos.Length;
    }
    
    //--------------------------------------------
    // On start
    void Start(){
        // Set max fps to 60
        Application.targetFrameRate = 60;
    }
    
    //--------------------------------------------
    // Every frame
    void Update(){
        // Update balls counter
        counter.text = spheresAmount.ToString();
    }
    
    //--------------------------------------------
    // Every fixed frame
    void FixedUpdate(){
        // Decrease time till next ball
        timeTillNext -= Time.deltaTime;
        // If time till next ball is smaller or equal 0 and there's less balls than max balls
        if(timeTillNext <= 0.0f && spheresAmount < MAX_BALLS){
            // Reset timer
            timeTillNext = NEXT_BALL_TIME;
            // Update balls counter
            counter.text = spheresAmount.ToString();
            // Prepare position of new ball
            float posX = Random.Range(0.05f, 0.95f);
            float posY = Random.Range(0.05f, 0.95f);
            Vector3 pos = new Vector3(posX, posY, 45.0f);
            // Reposition to world coords
            pos = Camera.main.ViewportToWorldPoint(pos);
            // Create and position new ball
            var obj = Instantiate(spherePrefab, pos, new Quaternion(0, 0, 0, 0));
            // Turn on collisions of new ball
            obj.GetComponent<Kulka>().collisionTurnedOn = true;
        }
    }
}
