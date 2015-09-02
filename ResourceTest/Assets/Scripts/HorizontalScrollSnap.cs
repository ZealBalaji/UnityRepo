using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class HorizontalScrollSnap : MonoBehaviour
{
	[Tooltip("the container the screens or pages belong to")]
	public Transform ScreensContainer;
	[Tooltip("how many screens or pages are there within the content")]
	private int Screens = 1;
	[Tooltip("which screen or page to start on")]
	public int StartingScreen = 1;
	
	private List<Vector3>     m_Positions;
	private ScrollRect         m_ScrollRect;
	private Vector3         m_LerpTarget;
	private bool             m_Lerp;
	private RectTransform    m_ScrollViewRectTrans;

	private Vector3 v3Pos;

	public static int index;
	public static int selectedShipIndex;

	public Image imagePrefab;
	//public List<Sprite> shipsList;
	private Image [] frame;
	private int frameCount;

	private Color normalColor;
	private Color highlitedColor;

	Sprite [] shipsList;

	void Start()
	{
		shipsList = (Sprite[]) Resources.LoadAll<Sprite>("ShipsTextures");

		frameCount=shipsList.Length;
		Screens=frameCount/2+1;
		frame=new Image[frameCount];

		normalColor=new Color(1,1,1,0.3f);
		highlitedColor=new Color(1,1,1,1);

		ScreensContainer.GetComponent<RectTransform>().sizeDelta=new Vector2(imagePrefab.rectTransform.sizeDelta.x*(frameCount+1),ScreensContainer.GetComponent<RectTransform>().sizeDelta.y);

		for (int i = 0, j=200; i <frameCount ; i++, j+=400)
		{
			Vector3 pos=new Vector3(j,0,0);
			
			frame[i]=Instantiate(imagePrefab,pos,imagePrefab.transform.rotation) as Image;
			frame[i].name=i.ToString();
			frame[i].transform.SetParent(ScreensContainer, false);
			frame[i].transform.GetChild(0).GetComponent<Image>().sprite=shipsList[i];
		}

		m_ScrollRect = gameObject.GetComponent<ScrollRect>();
		m_ScrollViewRectTrans = gameObject.GetComponent<RectTransform>();
		m_ScrollRect.inertia = false;
		m_Lerp = true;
		
		m_Positions = new List<Vector3>();
				
		if (Screens > 0 && frameCount>0)
		{
			Vector3 startPos = ScreensContainer.localPosition;
			Vector3 endPos =ScreensContainer.localPosition + Vector3.left * ((Screens - 1) * m_ScrollViewRectTrans.rect.width);
			
			for (int i = 0; i < frameCount; ++i)
			{
				float horiNormPos = ((float) i / (((float)(Screens - 1)))/2);
				// this does not seem to have an effect [Tested on Unity 4.6.0 RC 2]
				m_ScrollRect.horizontalNormalizedPosition = horiNormPos; 
				m_Positions.Add(Vector3.Lerp(startPos, endPos, horiNormPos));

				print(horiNormPos);
			} 

			index=StartingScreen-1;
			selectedShipIndex=index;
			m_LerpTarget=m_Positions[index];

		}
		
		// this does not seem to have an effect [Tested on Unity 4.6.0 RC 2]
		//m_ScrollRect.horizontalNormalizedPosition = (float)(StartingScreen-1 ) / ((float)((Screens - 1))/2);


		for(int i=0;i<frameCount;i++)
		{
			if(index ==i)
			{
				if(selectedShipIndex ==index)
				{
					frame[i].transform.GetChild(0).GetComponent<Image>().color=highlitedColor;
					frame[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(410,410);
				}
				else
				{
					frame[i].transform.GetChild(0).GetComponent<Image>().color=highlitedColor;
				}
			}
			else
			{
				frame[i].transform.GetChild(0).GetComponent<Image>().color=normalColor;
				frame[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(400,400);
			}
			
		}


	}
	
	void FixedUpdate()
	{
		if (m_Lerp)
		{
			ScreensContainer.localPosition = Vector3.Lerp(ScreensContainer.localPosition, m_LerpTarget, 5 * Time.deltaTime);

			if (Vector3.Distance(ScreensContainer.localPosition, m_LerpTarget) < 0.001f)
			{
				m_Lerp = false;
			}


		}

	}

	void Update()
	{

//		for(int i=0;i<frameCount;i++)
//		{
//			if(index ==i)
//				frame[i].transform.GetChild(0).GetComponent<Image>().color=highlitedColor;
//			else
//				frame[i].transform.GetChild(0).GetComponent<Image>().color=normalColor;
//
//		}



	}


	public void OnMouseDown() {
		v3Pos = Input.mousePosition;
	}
	
	/// <summary>
	/// Bind this to UnityEditor Event trigger Pointer Up
	/// </summary>
	public void DragEnd()
	{
		if (m_ScrollRect.horizontal)
		{
			Vector3 v3 = Input.mousePosition - v3Pos;
			v3.Normalize();
		
			float f = Vector3.Dot(v3, Vector3.right);
			if (f >= 0.5) 
			{
				if(index>0)
				{
					//Debug.Log("Right");
					m_Lerp = true;
					m_LerpTarget = m_Positions[(--index)];
				}
			}
			else if((f!=0))
			{
				if(index<m_Positions.Count-1)
				{
					//Debug.Log("Left");
					m_Lerp = true;
					m_LerpTarget = m_Positions[++index];
				}
			}
			else if(f==0)
			{
				var posInImage = new Vector2(Input.mousePosition.x,Input.mousePosition.y) - RectTransformUtility.WorldToScreenPoint(Camera.main,gameObject.GetComponent<RectTransform>().position);

				if(posInImage.x >-55 && posInImage.x < 55)
				{
					selectedShipIndex=index;
					//IOSManager.isShipTypeChanged=true;
					print("clicked  "+selectedShipIndex);
				}


			}

			for(int i=0;i<frameCount;i++)
			{
				if(index ==i)
				{
					if(selectedShipIndex ==index)
					{
						frame[i].transform.GetChild(0).GetComponent<Image>().color=highlitedColor;
						frame[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(410,410);
					}
					else
					{
						frame[i].transform.GetChild(0).GetComponent<Image>().color=highlitedColor;
					}
				}
				else
				{
					frame[i].transform.GetChild(0).GetComponent<Image>().color=normalColor;
					frame[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(400,400);
				}
				
			}

		}


	}
	
	/// <summary>
	/// Bind this to UnityEditor Event trigger Drag
	/// </summary>
	public void OnDrag()
	{
		m_Lerp = false;


	}


	Vector3 FindClosestFrom(Vector3 start, List<Vector3> positions)
	{
		Vector3 closest = Vector3.zero;
		float distance = Mathf.Infinity;
		
		foreach (Vector3 position in m_Positions)
		{
			if (Vector3.Distance(start, position) < distance)
			{
				distance = Vector3.Distance(start, position);                
				closest = position;
			}
		}
		
		return closest;
	}
}