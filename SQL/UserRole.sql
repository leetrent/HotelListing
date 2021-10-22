SELECT
	u.UserName
FROM
	AspNetUsers u
INNER JOIN 
	AspNetUserRoles ur ON ur.UserId = u.Id
INNER JOIN
	AspNetRoles r on r.Id = ur.RoleId AND r.Name = 'Administrator'