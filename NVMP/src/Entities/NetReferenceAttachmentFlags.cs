using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
	/// <summary>
	/// Specifies custom attachment flag behaviour for controlling how attachments are presented.
	/// </summary>
	[Flags]
	public enum AttachmentFlags
	{
		Invalid,

		/// <summary>
		/// Disables NiNode attachment and falls back to just direct world position updating each frame.
		/// </summary>
		NoNodeAttachment = (1 << 1),

		/// <summary>
		/// If no node attachment is in use, this disables direct rotation attachment entirely.
		/// </summary>
		NoDirectRotationAttachment = (1 << 2),

		/// <summary>
		/// If no node attachment is in use, this disables direct rotation attachment on the pitch axis, allowing
		/// the entity to move freely up and down.
		/// </summary>
		NoDirectPitchRotationAttachment = (1 << 3),

		/// <summary>
		/// If no node attachment is in use, this disables direct rotation attachment on the yaw axis, allowing
		/// the entity to move freely up and down.
		/// </summary>
		NoDirectYawRotationAttachment = (1 << 4),

		/// <summary>
		/// If no node attachment is in use, this disables direct position attachment entirely.
		/// </summary>
		NoDirectPositionAttachment = (1 << 5),

		/// <summary>
		/// If no node attachment is in use, this disables direct position attachment on the Z axis, allowing
		/// the entity to move freely up and down.
		/// </summary>
		NoDirectZPositionAttachment = (1 << 6)
	};

}
