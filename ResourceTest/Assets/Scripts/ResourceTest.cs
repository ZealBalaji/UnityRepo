using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceTest : MonoBehaviour {

	//public List<Sprite> shipsList; 

	Sprite [] textures;

	void Start() {

		textures = (Sprite[]) Resources.LoadAll<Sprite>("ShipsTextures");

		for(int i=0;i<textures.Length;i++)
		{
			print(textures[i].name);
		}
	}
}