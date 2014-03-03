using UnityEngine;
using System.Collections;


namespace CharacterController
{
	public class CharacterCollisionState2D
	{
		public bool right;
		public bool left;
		public bool above;
		public bool below;
		public bool becameGroundedThisFrame;
		
		
		public void reset()
		{
			right = left = above = below = becameGroundedThisFrame = false;
		}
		
		
		public override string ToString()
		{
			return string.Format( "[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}", right, left, above, below );
		}
	}
}
