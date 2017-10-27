﻿namespace TicTacCore.Processors
{
	/// <summary>
	///     A data processor that actually processes data.
	/// </summary>
	public interface IDataProcessorOrBuilder
	{
		/// <summary>
		///     Check whether the object is a builder or not.
		/// </summary>
		bool IsBuilder { get; }

		/// <summary>
		///     This method returns either a newly built data processor (if its a builder), or
		///     the same data processor.
		/// </summary>
		/// <returns>
		///     This method returns either a newly built data processor (if its a builder), or
		///     the same data processor.
		/// </returns>
		IDataProcessor Build();
	}
}