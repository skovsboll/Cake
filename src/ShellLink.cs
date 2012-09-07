using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cake
{
	/// <summary>
	/// Summary description for ShellLink.
	/// </summary>
	public class ShellLink : IDisposable
	{
		#region ComInterop for IShellLink

		#region IPersist Interface
		[ComImport()]
		[Guid("0000010C-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IPersist
		{
			[PreserveSig]
				//[helpstring("Returns the class identifier for the component object")]
			void GetClassID(out Guid pClassID);
		}
		#endregion

		#region IPersistFile Interface
		[ComImport()]
		[Guid("0000010B-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IPersistFile
		{
			// can't get this to go if I extend IPersist, so put it here:
			[PreserveSig]
			void GetClassID(out Guid pClassID);

			//[helpstring("Checks for changes since last file write")]		
			void IsDirty();

			//[helpstring("Opens the specified file and initializes the object from its contents")]		
			void Load(
				[MarshalAs(UnmanagedType.LPWStr)] string pszFileName, 
				uint dwMode);

			//[helpstring("Saves the object into the specified file")]		
			void Save(
				[MarshalAs(UnmanagedType.LPWStr)] string pszFileName, 
				[MarshalAs(UnmanagedType.Bool)] bool fRemember);

			//[helpstring("Notifies the object that save is completed")]		
			void SaveCompleted(
				[MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

			//[helpstring("Gets the current name of the file associated with the object")]		
			void GetCurFile(
				[MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
		}
		#endregion

		#region IShellLink Interface
		[ComImport()]
		[Guid("000214EE-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellLinkA
		{
			//[helpstring("Retrieves the path and filename of a shell link object")]
			void GetPath(
				[Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, 
				int cchMaxPath, 
				ref _WIN32_FIND_DATAA pfd, 
				uint fFlags);

			//[helpstring("Retrieves the list of shell link item identifiers")]
			void GetIDList(out IntPtr ppidl);

			//[helpstring("Sets the list of shell link item identifiers")]
			void SetIDList(IntPtr pidl);

			//[helpstring("Retrieves the shell link description string")]
			void GetDescription(
				[Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile,
				int cchMaxName);
		
			//[helpstring("Sets the shell link description string")]
			void SetDescription(
				[MarshalAs(UnmanagedType.LPStr)] string pszName);

			//[helpstring("Retrieves the name of the shell link working directory")]
			void GetWorkingDirectory(
				[Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir,
				int cchMaxPath);

			//[helpstring("Sets the name of the shell link working directory")]
			void SetWorkingDirectory(
				[MarshalAs(UnmanagedType.LPStr)] string pszDir);

			//[helpstring("Retrieves the shell link command-line arguments")]
			void GetArguments(
				[Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, 
				int cchMaxPath);

			//[helpstring("Sets the shell link command-line arguments")]
			void SetArguments(
				[MarshalAs(UnmanagedType.LPStr)] string pszArgs);

			//[propget, helpstring("Retrieves or sets the shell link hot key")]
			void GetHotkey(out short pwHotkey);
			//[propput, helpstring("Retrieves or sets the shell link hot key")]
			void SetHotkey(short pwHotkey);

			//[propget, helpstring("Retrieves or sets the shell link show command")]
			void GetShowCmd(out uint piShowCmd);
			//[propput, helpstring("Retrieves or sets the shell link show command")]
			void SetShowCmd(uint piShowCmd);

			//[helpstring("Retrieves the location (path and index) of the shell link icon")]
			void GetIconLocation(
				[Out(), MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, 
				int cchIconPath, 
				out int piIcon);
		
			//[helpstring("Sets the location (path and index) of the shell link icon")]
			void SetIconLocation(
				[MarshalAs(UnmanagedType.LPStr)] string pszIconPath, 
				int iIcon);

			//[helpstring("Sets the shell link relative path")]
			void SetRelativePath(
				[MarshalAs(UnmanagedType.LPStr)] string pszPathRel, 
				uint dwReserved);

			//[helpstring("Resolves a shell link. The system searches for the shell link object and updates the shell link path and its list of identifiers (if necessary)")]
			void Resolve(
				IntPtr hWnd, 
				uint fFlags);

			//[helpstring("Sets the shell link path and filename")]
			void SetPath(
				[MarshalAs(UnmanagedType.LPStr)] string pszFile);
		}


		[ComImport()]
		[Guid("000214F9-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellLinkW
		{
			//[helpstring("Retrieves the path and filename of a shell link object")]
			void GetPath(
				[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, 
				int cchMaxPath, 
				ref _WIN32_FIND_DATAW pfd, 
				uint fFlags);

			//[helpstring("Retrieves the list of shell link item identifiers")]
			void GetIDList(out IntPtr ppidl);

			//[helpstring("Sets the list of shell link item identifiers")]
			void SetIDList(IntPtr pidl);

			//[helpstring("Retrieves the shell link description string")]
			void GetDescription(
				[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
				int cchMaxName);
		
			//[helpstring("Sets the shell link description string")]
			void SetDescription(
				[MarshalAs(UnmanagedType.LPWStr)] string pszName);

			//[helpstring("Retrieves the name of the shell link working directory")]
			void GetWorkingDirectory(
				[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
				int cchMaxPath);

			//[helpstring("Sets the name of the shell link working directory")]
			void SetWorkingDirectory(
				[MarshalAs(UnmanagedType.LPWStr)] string pszDir);

			//[helpstring("Retrieves the shell link command-line arguments")]
			void GetArguments(
				[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, 
				int cchMaxPath);

			//[helpstring("Sets the shell link command-line arguments")]
			void SetArguments(
				[MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

			//[propget, helpstring("Retrieves or sets the shell link hot key")]
			void GetHotkey(out short pwHotkey);
			//[propput, helpstring("Retrieves or sets the shell link hot key")]
			void SetHotkey(short pwHotkey);

			//[propget, helpstring("Retrieves or sets the shell link show command")]
			void GetShowCmd(out uint piShowCmd);
			//[propput, helpstring("Retrieves or sets the shell link show command")]
			void SetShowCmd(uint piShowCmd);

			//[helpstring("Retrieves the location (path and index) of the shell link icon")]
			void GetIconLocation(
				[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, 
				int cchIconPath, 
				out int piIcon);
		
			//[helpstring("Sets the location (path and index) of the shell link icon")]
			void SetIconLocation(
				[MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, 
				int iIcon);

			//[helpstring("Sets the shell link relative path")]
			void SetRelativePath(
				[MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, 
				uint dwReserved);

			//[helpstring("Resolves a shell link. The system searches for the shell link object and updates the shell link path and its list of identifiers (if necessary)")]
			void Resolve(
				IntPtr hWnd, 
				uint fFlags);

			//[helpstring("Sets the shell link path and filename")]
			void SetPath(
				[MarshalAs(UnmanagedType.LPWStr)] string pszFile);
		}
		#endregion

		#region ShellLinkCoClass
		[Guid("00021401-0000-0000-C000-000000000046")]
		[ClassInterface(ClassInterfaceType.None)]
		[ComImport()]
		private class CShellLink{}

		#endregion
	
	private enum EShellLinkGP : uint
		{
			SLGP_SHORTPATH = 1,
			SLGP_UNCPRIORITY = 2
		}


		#region IShellLink Private structs

		[StructLayout(LayoutKind.Sequential, Pack=4, Size=0, CharSet=CharSet.Unicode)]
		private struct _WIN32_FIND_DATAW
		{
			public uint dwFileAttributes;
			public _FILETIME ftCreationTime;
			public _FILETIME ftLastAccessTime;
			public _FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr , SizeConst = 260)] // MAX_PATH
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[StructLayout(LayoutKind.Sequential, Pack=4, Size=0, CharSet=CharSet.Ansi)]
		private struct _WIN32_FIND_DATAA
		{
			public uint dwFileAttributes;
			public _FILETIME ftCreationTime;
			public _FILETIME ftLastAccessTime;
			public _FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr , SizeConst = 260)] // MAX_PATH
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[StructLayout(LayoutKind.Sequential, Pack=4, Size=0)]
		private struct _FILETIME 
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;
		}  
		#endregion	


		#region Enumerations
		/// <summary>
		/// Flags determining how the links with missing
		/// targets are resolved.
		/// </summary>
		[Flags]
		public enum EShellLinkResolveFlags : uint
		{
			/// <summary>
			/// Allow any match during resolution.  Has no effect
			/// on ME/2000 or above, use the other flags instead.
			/// </summary>
			SLR_ANY_MATCH = 0x2,
			/// <summary>
			/// Call the Microsoft Windows Installer. 
			/// </summary>
			SLR_INVOKE_MSI = 0x80,
			/// <summary>
			/// Disable distributed link tracking. By default, 
			/// distributed link tracking tracks removable media 
			/// across multiple devices based on the volume name. 
			/// It also uses the UNC path to track remote file 
			/// systems whose drive letter has changed. Setting 
			/// SLR_NOLINKINFO disables both types of tracking.
			/// </summary>
			SLR_NOLINKINFO = 0x40,
			/// <summary>
			/// Do not display a dialog box if the link cannot be resolved. 
			/// When SLR_NO_UI is set, a time-out value that specifies the 
			/// maximum amount of time to be spent resolving the link can 
			/// be specified in milliseconds. The function returns if the 
			/// link cannot be resolved within the time-out duration. 
			/// If the timeout is not set, the time-out duration will be 
			/// set to the default value of 3,000 milliseconds (3 seconds). 
			/// </summary>										    
			SLR_NO_UI = 0x1,
			/// <summary>
			/// Not documented in SDK.  Assume same as SLR_NO_UI but 
			/// intended for applications without a hWnd.
			/// </summary>
			SLR_NO_UI_WITH_MSG_PUMP = 0x101,
			/// <summary>
			/// Do not update the link information. 
			/// </summary>
			SLR_NOUPDATE = 0x8,
			/// <summary>
			/// Do not execute the search heuristics. 
			/// </summary>																																																																																																																																																																																																														
			SLR_NOSEARCH = 0x10,
			/// <summary>
			/// Do not use distributed link tracking. 
			/// </summary>
			SLR_NOTRACK = 0x20,
			/// <summary>
			/// If the link object has changed, update its path and list 
			/// of identifiers. If SLR_UPDATE is set, you do not need to 
			/// call IPersistFile::IsDirty to determine whether or not 
			/// the link object has changed. 
			/// </summary>
			SLR_UPDATE  = 0x4
		}

		#endregion

		#region Member Variables
		// Use Unicode (W) under NT, otherwise use ANSI		
		private IShellLinkW linkW;
		private IShellLinkA linkA;
		private string shortcutFile = "";
		#endregion

		#region Constructor
		/// <summary>
		/// Creates an instance of the Shell Link object.
		/// </summary>
		public ShellLink()
		{
			if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				linkW = (IShellLinkW)new CShellLink();
			}
			else
			{
				linkA = (IShellLinkA)new CShellLink();
			}
		}

		/// <summary>
		/// Creates an instance of a Shell Link object
		/// from the specified link file
		/// </summary>
		/// <param name="linkFile">The Shortcut file to open</param>
		public ShellLink(string linkFile) : this()
		{
			Open(linkFile);
		}
		#endregion

		#region Destructor and Dispose
		/// <summary>
		/// Call dispose just in case it hasn't happened yet
		/// </summary>
		~ShellLink()
		{
			Dispose();
		}

		/// <summary>
		/// Dispose the object, releasing the COM ShellLink object
		/// </summary>
		public void Dispose()
		{
			if (linkW != null ) 
			{
				Marshal.ReleaseComObject(linkW);
				linkW = null;
			}
			if (linkA != null)
			{
				Marshal.ReleaseComObject(linkA);
				linkA = null;
			}
		}
		#endregion

		#region Implementation
	
		/// <summary>
		/// Gets/sets the fully qualified path to the link's target
		/// </summary>
		public string Target
		{
			get
			{		
				StringBuilder target = new StringBuilder(260, 260);
				if (linkA == null)
				{
					_WIN32_FIND_DATAW fd = new _WIN32_FIND_DATAW();
					linkW.GetPath(target, target.Capacity, ref fd, (uint)EShellLinkGP.SLGP_UNCPRIORITY);
				}
				else
				{
					_WIN32_FIND_DATAA fd = new _WIN32_FIND_DATAA();
					linkA.GetPath(target, target.Capacity, ref fd, (uint)EShellLinkGP.SLGP_UNCPRIORITY);
				}
				return target.ToString();
			}
			set
			{
				if (linkA == null)
				{
					linkW.SetPath(value);
				}
				else
				{
					linkA.SetPath(value);
				}
			}
		}



		/// <summary>
		/// Loads a shortcut from the specified file
		/// </summary>
		/// <param name="linkFile">The shortcut file (.lnk) to load</param>
		public void Open(
			string linkFile			
			)
		{
			Open(linkFile, 
				IntPtr.Zero, 
				(EShellLinkResolveFlags.SLR_ANY_MATCH | EShellLinkResolveFlags.SLR_NO_UI),
				1);
		}
	

		/// <summary>
		/// Loads a shortcut from the specified file, and allows flags controlling
		/// the UI behaviour if the shortcut's target isn't found to be set.  If
		/// no SLR_NO_UI is specified, you can also specify a timeout.
		/// </summary>
		/// <param name="linkFile">The shortcut file (.lnk) to load</param>
		/// <param name="hWnd">The window handle of the application's UI, if any</param>
		/// <param name="resolveFlags">Flags controlling resolution behaviour</param>
		/// <param name="timeOut">Timeout if SLR_NO_UI is specified, in ms.</param>
		public void Open(
			string linkFile,
			IntPtr hWnd, 
			EShellLinkResolveFlags resolveFlags,
			ushort timeOut
			)
		{
			uint flags;

			if ((resolveFlags & EShellLinkResolveFlags.SLR_NO_UI) 
				== EShellLinkResolveFlags.SLR_NO_UI)
			{
				flags = (uint)((int)resolveFlags | (timeOut << 16));
			}
			else
			{
				flags = (uint)resolveFlags;
			}

			if (linkA == null)
			{
				((IPersistFile)linkW).Load(linkFile, 0); //STGM_DIRECT)
				linkW.Resolve(hWnd, flags);
				this.shortcutFile = linkFile;
			}
			else
			{
				((IPersistFile)linkA).Load(linkFile, 0); //STGM_DIRECT)
				linkA.Resolve(hWnd, flags);
				this.shortcutFile = linkFile;
			}
		}
		#endregion
	}
	#endregion

}
