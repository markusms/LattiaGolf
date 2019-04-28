using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using TMPro;
using UnityEngine.SceneManagement;

public class InstantiateObjectOnTouch : MonoBehaviour { 
	public Camera FirstPersonCamera; // The first-person camera being used to render the passthrough camera

    public GameObject ball;
    public GameObject ballNoArrow;
    public GameObject prefabArrow;
    public GameObject prefabArrowSmall;
    public GameObject PlaceGroundObject;
    public GameObject instanciatedBall;
    public GameObject arrow;
    public GameObject arrowSmall;
    public GameObject flag;
    public GameObject placedFlag;
    public GameObject placedGroundForFlag;
    public GameObject hole;

    public GameObject powerButtonText;
    public GameObject powerButtonTextNormal;
    public GameObject buttonText;
    public GameObject angleButtonTextNormal;
    public GameObject textEnding;
    public GameObject angleButtonText;
    public GameObject strokesText;
    public Button angleButtonLeft;
    public Button angleButtonRight;
    public Button buttonShoot;
    public Button buttonReset;
    public Button buttonNextLevel;
    public Button buttonTotalReset;
    public Button buttonPowerUp;
    public Button buttonPowerDown;

    public Texture2D grass;
    public Texture2D water;

    public Vector3 startPosition;
    public Vector3 flagPosition;

    public Ending endingScript;

    public bool placedGround = false;
    public bool placedBall = false;
    public bool ballCloseToFlag = false;
    private float zForce = 1f;
    private float yScale = 8;
    private float forceColor = 0.5f;
    private float power = 5f;
    private int angle = 0;
    private int scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene().buildIndex;
        if(scene != 5)
        {
            buttonNextLevel = GameObject.Find("ButtonNextLevel").GetComponent<Button>();
            buttonNextLevel.onClick.AddListener(() => NextLevelButtonClicked());
        }
        buttonShoot.onClick.AddListener(() => ShootButtonClicked());
        buttonReset.onClick.AddListener(() => ResetButtonClicked());
        buttonTotalReset.onClick.AddListener(() => TotalResetButtonClicked());
        buttonPowerUp.onClick.AddListener(() => PowerButtonClicked(1));
        buttonPowerDown.onClick.AddListener(() => PowerButtonClicked(0));
        angleButtonLeft.onClick.AddListener(() => AngleButtonClicked(0));
        angleButtonRight.onClick.AddListener(() => AngleButtonClicked(1));
        buttonText.SetActive(false);
        textEnding.SetActive(false);
        buttonShoot.gameObject.SetActive(false);
        buttonReset.gameObject.SetActive(false);
        buttonTotalReset.gameObject.SetActive(false);
        buttonPowerUp.gameObject.SetActive(false);
        buttonPowerDown.gameObject.SetActive(false);
        powerButtonText.gameObject.SetActive(false);
        angleButtonText.gameObject.SetActive(false);
        angleButtonLeft.gameObject.SetActive(false);
        angleButtonRight.gameObject.SetActive(false);
        endingScript = GameObject.FindObjectOfType<Ending>();
        if (scene == 5)
        {
            strokesText.GetComponent<TextMeshProUGUI>().text = "Strokes: "+ ScoreKeeper.strokes.ToString();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (scene == 5)
        {
            strokesText.GetComponent<TextMeshProUGUI>().text = "Strokes: " + ScoreKeeper.strokes.ToString();
        }
        if (arrow != null)
        {
            arrow.transform.position = instanciatedBall.transform.position;
            arrowSmall.transform.position = instanciatedBall.transform.position;
        }
        if(instanciatedBall != null)
        {
            if(instanciatedBall.GetComponent<Rigidbody>().velocity.x < 0.1 && instanciatedBall.GetComponent<Rigidbody>().velocity.z < 0.1)
            {
                arrow.SetActive(true);
            }
            else
            {
                arrow.SetActive(false);
            }
        }

        if(placedGround)
        {
            if (!ballCloseToFlag)
            {
                placedFlag.transform.position = new Vector3(hole.transform.position.x, hole.transform.position.y, hole.transform.position.z);
            }

            if(placedBall)
            {
                if(!ballCloseToFlag)
                {
                    if (Vector3.Distance(instanciatedBall.transform.position, hole.transform.position) < 2)
                    {
                        ballCloseToFlag = true;
                    }
                }
                else
                {
                    placedFlag.transform.position = Vector3.MoveTowards(placedFlag.transform.position, new Vector3(hole.transform.position.x, hole.transform.position.y+0.3f, hole.transform.position.z), Time.deltaTime);
                }
                
            }
        }

        //if(ball != null)
        //    arrow.GetComponent<Transform>().position = ball.transform.position;

        powerButtonText.GetComponent<TextMeshProUGUI>().text = power.ToString();
        angleButtonText.GetComponent<TextMeshProUGUI>().text = angle.ToString();
        powerButtonTextNormal.GetComponent<Text>().text = power.ToString();
        angleButtonTextNormal.GetComponent<Text>().text = angle.ToString();
        // Get the touch position from the screen to see if we have at least one touch event currently active
        Touch touch;
		if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
		{
			return;
		}
		// Now that we know that we have an active touch point, do a raycast to see if it hits
		// a plane where we can instantiate the object on.
		TrackableHit hit;
		var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

		if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && ball != null)
		{
            if(!placedGround || (scene == 5))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                // Instantiate a game object as a child of the anchor; its transform will now benefit
                // from the anchor's tracking.
                //var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);
                var placedObject = Instantiate(PlaceGroundObject, new Vector3(hit.Pose.position.x, hit.Pose.position.y + 0.05f, hit.Pose.position.z), hit.Pose.rotation);

                //Turn it 180 dgrees
                //Vector3 targetAngles = placedObject.transform.eulerAngles + 180f * Vector3.up; 
                //placedObject.transform.eulerAngles = targetAngles;
                if(scene!=5)
                    placedObject.transform.Translate(0, 0, 1.2f);

                //Each new cube will get a new color upon instantiation.
                placedObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
                // Make the newly placed object a child of the parent
                placedObject.transform.parent = anchor.transform;

                placedGroundForFlag = placedObject;
                hole = GameObject.FindGameObjectWithTag("Hole");
                flagPosition = hole.transform.position; //hole position
                placedFlag = Instantiate(flag, flagPosition, placedObject.transform.rotation);

                placedGround = true;
            }
            else if (!placedBall)
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                // Instantiate a game object as a child of the anchor; its transform will now benefit
                // from the anchor's tracking.
                //var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);
                var placedObject = Instantiate(ballNoArrow, new Vector3(hit.Pose.position.x, hit.Pose.position.y + 0.2f, hit.Pose.position.z), hit.Pose.rotation);
                startPosition = placedObject.transform.position;
                var placedArrow = Instantiate(prefabArrow, placedObject.transform.position, placedObject.transform.rotation);
                var placedArrowSmall = Instantiate(prefabArrow, placedObject.transform.position, placedObject.transform.rotation);
                placedArrow.transform.Rotate(90, 0, 0);
                placedArrow.transform.localScale = new Vector3(1, 8, 5);

                //Each new cube will get a new color upon instantiation.
                placedObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
                // Make the newly placed object a child of the parent
                placedObject.transform.parent = anchor.transform;
                placedBall = true;
                instanciatedBall = placedObject;
                //arrow = GameObject.Find("Arrow");
                arrow = placedArrow;
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, forceColor, forceColor);
                arrowSmall = placedArrowSmall;
                arrowSmall.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                if(scene != 5)
                {
                    buttonShoot.gameObject.SetActive(true);
                    buttonReset.gameObject.SetActive(true);
                    buttonTotalReset.gameObject.SetActive(true);
                    buttonPowerUp.gameObject.SetActive(true);
                    buttonPowerDown.gameObject.SetActive(true);
                    powerButtonText.gameObject.SetActive(true);
                    angleButtonText.gameObject.SetActive(true);
                    angleButtonLeft.gameObject.SetActive(true);
                    angleButtonRight.gameObject.SetActive(true);
                }
                endingScript.FindScript();
            }
			
		}
	}

    void PowerButtonClicked(int i)
    {
        if (i == 1)
        {
            if (power < 15)
            {
                power++;
                yScale += 0.2f;
                forceColor -= 0.1f;
                //GetComponent<Transform>().localScale = new Vector3(3, yScale, 1);
                //GetComponent<SpriteRenderer>().color = new Color(1, forceColor, forceColor);
                arrow.GetComponent<Transform>().localScale = new Vector3(1, yScale, 5);
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, forceColor, forceColor);
            }
        }
        else
        {
            if(power > 0)
            {
                power--;
                yScale -= 0.2f;
                forceColor += 0.1f;
                arrow.GetComponent<Transform>().localScale = new Vector3(1, yScale, 5);
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, forceColor, forceColor);
            }
        }
    }

    void ShootButtonClicked()
    {
        //instanciatedBall.GetComponent<Rigidbody>().AddRelativeForce(0, 0, zForce * power);
        ScoreKeeper.strokes += 1;
        instanciatedBall.GetComponent<Rigidbody>().AddForce(arrowSmall.transform.forward * zForce * power, ForceMode.Impulse);
    }

    void NextLevelButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void ResetButtonClicked()
    {
        instanciatedBall.transform.position = startPosition;
        instanciatedBall.transform.rotation = new Quaternion(0, 0, 0, 0);
        instanciatedBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        instanciatedBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void TotalResetButtonClicked()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    void AngleButtonClicked(int i)
    {
        if (i == 1)
        {
            angle++;
            //instanciatedBall.transform.Rotate(0, 10, 0);
            arrow.transform.Rotate(0, 0, -10);
            arrowSmall.transform.Rotate(0, 10, 0);
        }
        else
        {
            //instanciatedBall.transform.Rotate(0, -10, 0);
            arrow.transform.Rotate(0, 0, 10);
            arrowSmall.transform.Rotate(0, -10, 0);
            angle--;
        }
    }

    public void gameEnded()
    {
        textEnding.SetActive(true);
    }

}
