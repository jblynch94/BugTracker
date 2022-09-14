﻿using BugTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace BugTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Company> Companies { get; set; } = default!;
        public DbSet<Invite> Invite { get; set; } = default!;
        public DbSet<Project> Projects { get; set; } = default!;
        public DbSet<Notification> Notification { get; set; } = default!;
        public DbSet<NotificationType> NotificationTypes { get; set; } = default!;
        public DbSet<ProjectPriority> ProjectPriorities { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<TicketAttachment> TicketAttachment { get; set; } = default!;
        public DbSet<TicketComment> TicketComment { get; set; } = default!;
        public DbSet<TicketHistory> TicketHistory { get; set; } = default!;
        public DbSet<TicketPriority> TicketPriorities { get; set; } = default!;
        public DbSet<TicketStatus> TicketStatuses { get; set; } = default!;
        public DbSet<TicketType> TicketTypes { get; set; } = default!;
    }
}