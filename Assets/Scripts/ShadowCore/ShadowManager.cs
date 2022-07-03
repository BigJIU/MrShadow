using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class ShadowManager : MonoBehaviour
{
    private List<Transform> activateLightList;
    private Dictionary<Transform, Transform> refDic;
    //Instance 
    private static ShadowManager Instance;
    public static ShadowManager getInstance { get { return Instance; } }

    public GameObject shadowPrefab;
    public Transform LightObjects;
    public GameObject pointLight;
    public GameObject Player;
    private float canvasScale = 100f;
    private Vector3 lightPosi;

    private Dictionary<Transform, List<Vector2>> normalVertices;

    [Header("ShadowRelated Parameter")] 
    public float defaultOffsetX = 0f;
    public float defaultOffsetY = 1f;
    public float defaultXScale = 1f;
    public float defaultYScale = 1f;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        refDic = new Dictionary<Transform, Transform>();
        activateLightList = new List<Transform>();
        normalVertices = new Dictionary<Transform, List<Vector2>>();
        //Light, Shadow
        for (int i = 0; i < LightObjects.childCount; i++)
        {
            createShadow(LightObjects.GetChild(i));
        }
    }

    void Start()
    {


        lightPosi = pointLight.transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        templateLightMove();
        foreach (Transform light in activateLightList)
        {
            refDic[light].transform.position = light.position; 
        }
        
        if (pointLight.transform.position != lightPosi||Input.GetAxis("Horizontal")!=0)//Input.GetKeyDown(KeyCode.O))
        {
            foreach (Transform light in activateLightList)
            {
                updateShadow(light);
            }

            lightPosi = pointLight.transform.position;
        }

    }

    private void templateLightMove()
    {
        float movespeed = 1f;
        if (Input.GetKey(KeyCode.W))//无脑的移动测试
        {
            pointLight.transform.Translate(Vector3.up * movespeed * Time.deltaTime);         
        }
 
        if (Input.GetKey(KeyCode.S))
        {
            pointLight.transform.Translate(Vector3.down * movespeed * Time.deltaTime);
        
        }
 
        if (Input.GetKey(KeyCode.A) )
        {
            pointLight.transform.Translate(Vector3.left * movespeed * Time.deltaTime);
           
        }
 
        if (Input.GetKey(KeyCode.D))
        {
            pointLight.transform.Translate(Vector3.right * movespeed * Time.deltaTime);
            
        }
    }

    public void createShadow(Transform lightTransform)
    {
        GameObject tmpShadow = Instantiate(shadowPrefab, transform);
        tmpShadow.name = lightTransform.name;
        tmpShadow.transform.position = lightTransform.position;
        refDic.Add(lightTransform,tmpShadow.transform);
        Sprite lightSprite = lightTransform.gameObject.GetComponent<SpriteRenderer>().sprite;
        
        tmpShadow.GetComponent<ShapeImage>().sprite = lightSprite;

        tmpShadow.GetComponent<RectTransform>().sizeDelta = new Vector2(lightSprite.rect.width,lightSprite.rect.height);
        createShadowCollider(lightTransform,lightSprite.rect.height/2);
        
        //activateLightList.Add(lightTransform);
    }

    private void createShadowCollider(Transform lightTransform,float offset)
    {
        EdgeCollider2D lightCollider = lightTransform.GetComponent<EdgeCollider2D>();
        List<Vector2> vertices = new List<Vector2>();
        lightCollider.GetPoints(vertices);
        //foreach (var ver in vertices) Debug.Log(ver.ToString());
        
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] *= new Vector2(canvasScale, canvasScale);
            vertices[i] += new Vector2(0, offset);
        }
        normalVertices.Add(lightTransform,vertices);
        bool tryset = refDic[lightTransform].GetComponent<EdgeCollider2D>().SetPoints(vertices);
        //Debug.Log(tryset);
        //foreach (var ver in vertices) Debug.Log(ver.ToString());
    }


    
    public void updateShadow(Transform lightTransform)
    {
        //calculate transform vector
        float diffX = lightTransform.position.x - pointLight.transform.position.x - defaultOffsetX;
        float diffY = pointLight.transform.position.y - lightTransform.position.y - defaultOffsetY;
        
        Vector2 tranformVector = new Vector2(diffX * defaultXScale, diffY * defaultYScale);
        
        colliderTransform(tranformVector,lightTransform);
        imageTransform(tranformVector,lightTransform);
        //do transform in collider
        //do transform in image
    }

    private void colliderTransform(Vector2 tranformVector,Transform lightTransform)
    {
        
        EdgeCollider2D shadowCollider = refDic[lightTransform].GetComponent<EdgeCollider2D>();
        List<Vector2> vertices = new List<Vector2>( normalVertices[lightTransform].ToArray());
        //     new List<Vector2>();
        // shadowCollider.GetPoints(vertices);
        //tranformVector *= canvasScale;
        Vector3 boundSize = shadowCollider.bounds.size;
        float ymin = 10000f;
        foreach (var ver in vertices)
            if (ver.y < ymin)
                ymin = ver.y;
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = new Vector2(vertices[i].x-tranformVector.x*((boundSize.y-vertices[i].y)/boundSize.y)*(1+tranformVector.y)*(1+tranformVector.y), //+ymin
                vertices[i].y*(1+tranformVector.y));//*(1+tranformVector.y)
        }
        
        bool tryset = refDic[lightTransform].GetComponent<EdgeCollider2D>().SetPoints(vertices);
    }

    private void imageTransform(Vector2 tranformVector, Transform lightTransform)
    {
        ShapeImage shapeImage = refDic[lightTransform].GetComponent<ShapeImage>();
        Sprite tmpS = Sprite.Instantiate(shapeImage.sprite);
        
        shapeImage.sprite = tmpS;
        shapeImage.offset = tranformVector.x * canvasScale * (1+tranformVector.y);
        Debug.Log("x:"+ tranformVector.x);
        Debug.Log("y:"+ (1+tranformVector.y));
        refDic[lightTransform].GetComponent<RectTransform>().sizeDelta = new Vector2(tmpS.rect.width,tmpS.rect.height*(1+tranformVector.y));
        //refDic[lightTransform].localScale = new Vector3(1,- 1 - tranformVector.y/defaultYScale,1);
    }
    
    
    
    
    
    
    public void reverseShadow(Transform lightTransform)
    {
        Transform shadowTransform = refDic[lightTransform];
        //Fade off Shift shadow
        //Kill Old shadow
        //createShadow(lightTransform);
        //Fade in new Shadow
    }
    public void addShadowList(Transform shadowTransform)
    {
        activateLightList.Add(shadowTransform);
    }
    
}
