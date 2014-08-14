Public Class Enumerations

	Enum BudgetStatus
		Unknown = -1
		Active = 1
		Deleted = 101
	End Enum

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
		Contributor = 3
		Viewer = 4
	End Enum

	Enum ObjectType
		Unknown = -1
		Connector = 0
		User = 1
		Entry = 2
		Location = 3
		Category = 4
		Budget = 5
	End Enum

	Enum UserType
		Unknown = -1
		Admin = 1
		Editor = 2
		Contributor = 3
		Viewer = 4
	End Enum

	Enum UserStatus
		Unknown = -1
		Active = 1
		Banned = 4
		Deleted = 101
	End Enum

End Class
