namespace PixelBot.ResolverActors
{
	public static class ObjectArrayExtensions
	{

		/// <summary>
		/// Prepends newArgs to an existingArgs and returns it.
		/// </summary>
		/// <param name="existingArgs">existing object[]</param>
		/// <param name="newArgs">object[] to prepend to the existingArgs</param>
		/// <returns>resulting object[]</returns>
		public static object[] BuildArgs(this object[] existingArgs, params object[] newArgs)
		{
			var arguments = newArgs;
			if (existingArgs.Length > 0)
			{
				int newArgsLength = arguments.Length;
				int argLength = existingArgs.Length + newArgsLength;
				arguments = new object[argLength];
				newArgs.CopyTo(arguments, 0);
				existingArgs.CopyTo(arguments, newArgsLength);
			}
			return arguments;
		}
	}
}
