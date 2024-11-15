﻿using System;
using System.Collections.Generic;

namespace Sisus
{
	public static class DrawerPool
	{
		private const int MaxItemsOfSameType = 250;
		private static readonly Dictionary<Type, Stack<IDrawer>> DrawersPool = new Dictionary<Type,Stack<IDrawer>>(50);

		public static int Count => DrawersPool.Count;

        public static bool TryGet<T>(out T result) where T : class, IDrawer
		{
			Stack<IDrawer> stack;
			if(DrawersPool.TryGetValue(typeof(T), out stack))
			{
				if(stack.Count > 0)
				{
					result = stack.Pop() as T;
					return true;
				}
			}
			result = null;
			return false;
		}

		public static bool TryGet(Type type, out IDrawer result)
		{
			Stack<IDrawer> stack;
			if(DrawersPool.TryGetValue(type, out stack))
			{
				if(stack.Count > 0)
				{
					result = stack.Pop();
					return true;
				}
			}
			result = null;
			return false;
		}

		public static void Pool(IDrawer disposing)
		{
			var type = disposing.GetType();

			Stack<IDrawer> stack;
			if(!DrawersPool.TryGetValue(type, out stack))
			{
				stack = new Stack<IDrawer>();
				DrawersPool[type] = stack;
			}

			#if DEV_MODE || SAFE_MODE
			if(stack.Contains(disposing))
			{
				UnityEngine.Debug.LogError("PolymorphicPool.Dispose was called for item " + StringUtils.ToString(disposing) + " of type "+ StringUtils.ToStringSansNamespace(type) + " but pool already contained the same item!");
				return;
			}
			#endif

			int count = stack.Count;
			if(count >= MaxItemsOfSameType)
			{
				#if DEV_MODE
				UnityEngine.Debug.LogWarning("PolymorphicPool.Dispose was called for item " + StringUtils.ToString(disposing) + " of type " + StringUtils.ToStringSansNamespace(type) + " but pool already contained "+ count + " instances of the same item!");
				#endif
				return;
			}
			
			stack.Push(disposing);

			#if DEV_MODE && DEBUG_POOLED_COUNT
			UnityEngine.Debug.Log("PolymorphicPool.Dispose was called for item " + StringUtils.ToString(disposing) + " of type " + StringUtils.ToString(typeof(T)) + ". Pool now contains "+ stack.Count + " instances of said type.");
			#endif
		}

		public static string List()
		{
			return StringUtils.ToString(DrawersPool, "\n");
		}
	}
}