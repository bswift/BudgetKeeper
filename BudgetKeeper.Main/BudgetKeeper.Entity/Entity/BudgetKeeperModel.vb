Public Class BudgetKeeperModel
	Private m_EntitiesInstance As BudgetKeeperEntity.Entities

	Public Property BudgetKeeperEntity As Entities
		Get
			Return m_EntitiesInstance
		End Get
		Set(ByVal value As Entities)
			m_EntitiesInstance = value
		End Set
	End Property

	Public Sub New(ByVal ConnectionString As String)
		Dim ecStringBuild As System.Data.EntityClient.EntityConnectionStringBuilder = New System.Data.EntityClient.EntityConnectionStringBuilder
		ecStringBuild.Provider = "System.Data.SqlClient"
		ecStringBuild.ProviderConnectionString = ConnectionString
		ecStringBuild.Metadata = "res://*/Entity.BudgetKeeperModel.csdl|res://*/Entity.BudgetKeeperModel.ssdl|res://*/Entity.BudgetKeeperModel.msl"

		m_EntitiesInstance = New Entities(ecStringBuild.ToString())
		m_EntitiesInstance.Database.CommandTimeout = 500
	End Sub

End Class
