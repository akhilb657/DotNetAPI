USE DotNetCourseDatabase
GO

ALTER PROCEDURE TutorialAppSchema.spUsers_Get
/*EXEC TutorialAppSchema.spUsers_Get @UserId = 4*/
    @UserId INT = NULL
AS 
BEGIN
    SELECT [Users].[UserId],
    [Users].[FirstName],
    [Users].[LastName],
    [Users].[Email],
    [Users].[Gender],
    [Users].[Active],
    UserSalary.Salary,
    UserJobInfo.Department,
    UserJobInfo.JobTitle,
    AvgSalary.AvgSalary
    FROM TutorialAppSchema.Users AS Users
        LEFT JOIN TutorialAppSchema.UserSalary AS UserSalary
            ON UserSalary.UserId = Users.UserId  
        LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
            ON UserJobInfo.UserId = Users.UserId
        OUTER APPLY (SELECT AVG(UserSalary2.Salary) AvgSalary
            FROM TutorialAppSchema.Users AS Users
                LEFT JOIN TutorialAppSchema.UserSalary AS UserSalary2
                    ON UserSalary2.UserId = Users.UserId  
                LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo2
                    ON UserJobInfo2.UserId = Users.UserId
                WHERE UserJobInfo2.Department = UserJobInfo.Department
                GROUP BY UserJobInfo2.Department) AS AvgSalary
        WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
END