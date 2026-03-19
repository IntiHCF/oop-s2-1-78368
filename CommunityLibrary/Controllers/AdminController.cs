using Microsoft.AspNetCore.Identity; // For roles
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // For [Authorize]

[Authorize(Roles = "Admin")] // Only Admins can access
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult Roles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
        return RedirectToAction("Roles");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null) await _roleManager.DeleteAsync(role);
        return RedirectToAction("Roles");
    }
}