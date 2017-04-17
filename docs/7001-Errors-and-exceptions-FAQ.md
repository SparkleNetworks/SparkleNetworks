
Errors and exceptions FAQ
================================



InvalidOperationException: There is already an open DataReader associated with this Command which must be closed first.
-------------------------------------------------------------------

You are:

* writing a domain method
* which uses a data transaction
* you are calling a stored procedure
* this stored procedure is mapped in the Entity Framework model
* and you call the `context.MyProcedure()` method.
* The next data operation (be it a get or a commit) fails with the hell exception.

Resolution:

**Entity Framework fails at executing stored procedures without breaking the transaction.** Make a low-level SQL call to the stored procedure instead.

Exemple: 

* I (SandRock) was writing `JobService.Delete()`.
* I created the `dbo.DeleteJob` stored procedure, mapped it into the EF model.

This code failed:

```
tran.Repositories.Job.DeleteJob(request.Id, request.TargetJobId); // THE SP CALL
tran.CompleteTransaction(); // THE exception
```

This one too:

```
tran.Repositories.Job.DeleteJob(request.Id, request.TargetJobId); // THE SP CALL
var deleteJob1 = tran.Repositories.Job.GetById(request.Id); // THE exception
tran.CompleteTransaction();
```

Because in the EF repository I was doing:

```
public void DeleteJob(int deleteJobId, int? targetJobId)
{
    // Entity Framework fails to execute the SP without failing the transaction
    var result = this.Context.DeleteJob(deleteJobId, targetJobId);
}
```

I fixed it this way:

```
public void DeleteJob(int deleteJobId, int? targetJobId)
{
    // We call the SP manually to ensure the transaction does not get broken
    DeleteJob_Result result = null;

    var commandText = "EXEC dbo.DeleteJob @deleteJobId = " + deleteJobId + " ";
    var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
        .SetText(commandText);

    using (var reader = cmd.ExecuteReader())
    {
        if (reader.Read())
        {
            result = new DeleteJob_Result();
            result.AffectedUsers = reader.GetInt32(reader.GetOrdinal("AffectedUsers"));
            result.DeletedJobs = reader.GetInt32(reader.GetOrdinal("DeletedJobs"));
        }
    }
}
```

Note that the internet does not reference this issue at all.











