using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verlet_rope : MonoBehaviour
{

    public struct rope {
        public Vector2 new_pos, old_pos ;

        public rope(Vector2 pos) {
            this.new_pos = this.old_pos = pos;
        }
    }

    LineRenderer line;
    List<rope> segment = new List<rope>();
    float gap;
    float width;

    void Awake() {
        gap = 0.15f;
        width = 0.1f;
    }

    void Start() {
        
        line = GetComponent<LineRenderer>();
        Vector3 startpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        for (int i = 0; i < 50; i++) {
            segment.Add(new rope(startpoint));
            startpoint.y -= gap;
        }

    }

    private void draw_rope() {
        line.startWidth = line.endWidth = width;
        Vector3[] rope_position = new Vector3[50];

        for (int i = 0; i < 50; i++)
            rope_position[i] = segment[i].new_pos;    

        line.positionCount = rope_position.Length;
        line.SetPositions(rope_position);

    }

    void Update() {
        draw_rope();
    }

    void FixedUpdate() {
        simulate();
        /*for (int i = 0; i < 50; i++)
            constraints();*/
    
    }

    void simulate() {

        Vector2 gravity = new Vector2( 0 , -10 );

        for (int i = 1; i < 50; i++) {
            
            rope first = segment[i];
            
            Vector2 velocity = first.new_pos - first.old_pos;
            
            first.old_pos = first.new_pos;
            first.new_pos += velocity * 10 * Time.deltaTime;
            first.new_pos += gravity * Time.deltaTime;
            segment[i] = first; 

        }

        for (int i = 0; i < 50; i++)
            constraints();

    }

    void constraints() {

        rope first = segment[0];
        first.new_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        segment[0] = first;

        for (int i = 0; i < 49; i++) {

            rope firstseg = segment[i];
            rope secondseg = segment[i + 1];
            
            float dist = (firstseg.new_pos - secondseg.new_pos).magnitude;
            float error = Mathf.Abs(dist - gap);
            Vector2 changedir = Vector2.zero;

            if(dist > gap)
                changedir = (firstseg.new_pos - secondseg.new_pos).normalized;
            else if(dist < gap)
                    changedir = (secondseg.new_pos - firstseg.new_pos).normalized;
            
            Vector2 changeamount = changedir * error;
            
            if(i != 0) {
                
                firstseg.new_pos -= changeamount * error;
                segment[i] = firstseg;
                secondseg.new_pos += changeamount * 0.5f;
                segment[i + 1] = secondseg;

            }
            else {

                secondseg.new_pos += changeamount;
                segment[i + 1] = secondseg; 

            }   

        }

    }
   
}
