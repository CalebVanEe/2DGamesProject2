using UnityEngine;
using UnityEngine.UI;
public class LivesScript : MonoBehaviour
{
    public GameObject heartPrefab;
    public float spacing;

    private Image[] hearts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int numLives = PlayerPrefs.GetInt("Lives");
        hearts = new Image[numLives];
        
        for (int i = 0; i < numLives; i++) {
            Image newHeart = Instantiate(heartPrefab, transform).GetComponent<Image>();
            newHeart.rectTransform.anchoredPosition = new Vector2(i * spacing, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
