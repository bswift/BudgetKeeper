Public Class Enumerations

	Enum CategoryStatus
		Unknown = -1
		Active = 1
		Deleted = 101
	End Enum

	Enum EntryStatus
		Unknown = -1
		Active = 1
		Deleted = 101
	End Enum

	Enum EntryType
		Unknown = -1
		Generic = 1
	End Enum

	Enum LocationStatus
		Unknown = -1
		Active = 1
		Deleted = 101
	End Enum

	Enum LocationType
		Unknown = -1
		Generic = 1
	End Enum

	Enum LoginType
		Unknown = -1
		Admin = 1
		Editor = 2
		Viewer = 3
	End Enum

	Enum ObjectType
		Unknown = -1
		Connector = 0
		User = 1
		Entry = 2
		Location = 3
		Category = 4
	End Enum

	Enum UserType
		Unknown = -1
		Admin = 1
		Editor = 2
		Viewer = 3
	End Enum

	Enum UserStatus
		Unknown = -1
		Active = 1
		Banned = 4
		Deleted = 101
	End Enum

End Class
