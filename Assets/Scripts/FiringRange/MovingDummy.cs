using System;
using TMPro;
using UnityEngine;

namespace FiringRange
{
	public class MovingDummy : BaseDummy
	{
		[Header("Movement")]
		[SerializeField] private Transform pointA;
		[SerializeField] private Transform pointB;
		[SerializeField] private float moveSpeed = 2;

		private Vector3 targetPos;

		protected override void OnActivated()
		{
			transform.position = pointA.position;
			targetPos = pointB.position;
		}

		private void FixedUpdate()
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

			if (Vector3.Distance(transform.position, targetPos) < 0.1f)
			{
				targetPos = (targetPos == pointA.position) ? pointB.position : pointA.position;
			}
		}
	}
}