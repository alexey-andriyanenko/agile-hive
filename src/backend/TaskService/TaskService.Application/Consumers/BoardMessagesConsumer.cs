using BoardMessages.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TaskService.Infrastructure;
using TaskService.Infrastructure.Data;
using TenantContextService.Contracts;

namespace TaskService.Application.Consumers;

public class BoardMessagesConsumer(ILogger<BoardMessagesConsumer> logger, TenantContextService.Contracts.TenantContextService.TenantContextServiceClient tenantContextServiceClient, IPublishEndpoint publishEndpoint, IMemoryCache memoryCache)
    : IConsumer<BoardCreatedMessage>
{
    private const string ServiceName = "TaskService";
  
    public async Task Consume(ConsumeContext<BoardCreatedMessage> context)
    {
      var message = context.Message;
      
      var cacheKey = $"tenantcontext:{context.Message.OrganizationId}:{ServiceName}";
      
      var tenantContextResult = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
        
        var resp = await tenantContextServiceClient.GetTenantContextAsync(new GetTenantContextRequest
        {
          TenantId = message.OrganizationId.ToString(),
          ServiceName = ServiceName
        }).ResponseAsync;

        return new TenantContext()
        {
          TenantId = context.Message.OrganizationId,
          DbConnectionString = resp.DbConnectionString,
          ServiceName = ServiceName
        };
      });
      
      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
      optionsBuilder.UseNpgsql(tenantContextResult!.DbConnectionString);
        
      var tenantContext = new TenantContext()
      {
        TenantId = context.Message.OrganizationId,
        ServiceName = "TaskService",
        DbConnectionString = tenantContextResult.DbConnectionString,
      };
        
      await using var db = new ApplicationDbContext(optionsBuilder.Options, tenantContext);
      
        var tagIds = new List<Guid>()
        {
            Guid.Parse("0a1b2c3d-4e5f-4a6b-8c7d-1e2f3a4b5c6d"),
            Guid.Parse("2c3d4e5f-6a7b-4c8d-8e9f-3a4b5c6d7e8f"),
            Guid.Parse("3d4e5f6a-7b8c-4d9e-8f0a-4b5c6d7e8f90"),
            Guid.Parse("4e5f6a7b-8c9d-4e0f-8a1b-5c6d7e8f9012"),
            Guid.Parse("8c9d0e1f-2a3b-4c4d-8e5f-90123456789a"),
            Guid.Parse("5f6a7b8c-9d0e-4f1a-8b2c-6d7e8f901234"),
        };

        var descriptions = new List<(string Json, string Plain)>
        {
            (
                // Description 1: User flows, links, examples, QA notes
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Acceptance criteria and flows"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Main flows and examples for signup, login and profile."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"/register should accept firstname, lastname, username, email, password"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"confirmation email contains link (https://example.com/confirm) and expires in 24h"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"/login should allow login by username or email; account lockout after 5 failed attempts"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"QA notes: include cases for unicode, very long names and missing fields."}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Example request:"}]},
                    {"type":"codeBlock","attrs":{"language":"json"},"content":[{"type":"text","text":"{ \"email\": \"test@example.com\", \"password\": \"P@ssw0rd!\" }"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Related tickets: #1234, #1250"}]}
                  ]
                }
                """,
                "Acceptance criteria and flows:\n- /register should accept firstname, lastname, username, email, password\n- confirmation email contains link (https://example.com/confirm) and expires in 24h\n- /login allows username or email; lockout after 5 failed attempts\n\nQA notes: test unicode, long names, missing fields\nExample request: { \"email\": \"test@example.com\", \"password\": \"P@ssw0rd!\" }\nRelated tickets: #1234, #1250"
            ),
            (
                // Description 2: Admin, roles, audit, RBAC matrix, examples
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Admin & Roles"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Role-based access control (RBAC) and audit requirements."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"admin can assign roles to users via /admin/users/{id}/roles"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"role changes must be logged to audit service with actor, timestamp and diff"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"RBAC matrix attached: Viewer, Editor, Admin, Billing"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Security considerations: validate role input, prevent privilege escalation."}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Audit page: /admin/audit (supports filters: user, action, date-range)"}]}
                  ]
                }
                """,
                "Admin & Roles:\n- admin can assign roles via /admin/users/{id}/roles\n- role changes logged with actor, timestamp and diff\n- RBAC: Viewer, Editor, Admin, Billing\n\nSecurity: validate role input, prevent privilege escalation\nAudit page: /admin/audit (filters: user, action, date-range)"
            ),
            (
                // Description 3: API, rate limits, pagination, examples, SDK note
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"API & SDK"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Public API contract, rate limits and SDK guidance."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"All endpoints require an API key header: X-API-Key"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Rate limit: 100 requests/min per key; burst allowed up to 200 with token bucket"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Pagination uses cursor-based tokens; examples in SDK"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Error handling: 4xx for client errors; 5xx for server errors; include error_code and details."}]},
                    {"type":"paragraph","content":[{"type":"text","text":"SDK: ensure retry logic for 429 and idempotency for POST operations."}]}
                  ]
                }
                """,
                "API & SDK:\n- All endpoints require header X-API-Key\n- Rate limit: 100 req/min per key; burst to 200 with token bucket\n- Pagination: cursor-based tokens\n\nError handling: 4xx client, 5xx server; include error_code and details\nSDK: retry on 429; make POST idempotent where possible"
            ),
            (
                // Description 4: UX, redirects, localization, accessibility
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"UX, Localization & Accessibility"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"UX flows, redirects and accessibility requirements."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"after login redirect to dashboard (https://example.com/dashboard); support returnTo param"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"show onboarding modal for new users with skip option and analytics event"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"support i18n keys and fallback to en-US; include RTL support for ar, he"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Accessibility: all interactive elements must be keyboard reachable and have aria-labels."}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Performance: lazy-load images and defer non-critical JS."}]}
                  ]
                }
                """,
                "UX, Localization & Accessibility:\n- redirect to dashboard with returnTo param\n- onboarding modal with skip and analytics\n- i18n keys with en-US fallback; RTL support for ar, he\n\nAccessibility: keyboard reachable, aria-labels\nPerformance: lazy-load images, defer non-critical JS"
            ),
            (
                // Description 5: Resilience, retries, logging, observability
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Resilience & Observability"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Guidelines for retries, logging and monitoring."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Transient errors retried 3 times with exponential backoff (base 500ms, max 8s)"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Structured logging with request_id, user_id and correlation_id"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Metrics: request_latency, error_rate, active_users; alert on 5xx spike"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Tracing: propagate trace headers across services (traceparent)."}]}
                  ]
                }
                """,
                "Resilience & Observability:\n- Retry transient errors 3 times with exponential backoff (500ms base, max 8s)\n- Structured logs with request_id, user_id, correlation_id\n- Metrics: request_latency, error_rate, active_users; alert on 5xx spike\nTracing: propagate 'traceparent' across services"
            ),
            (
                // Description 6: Performance, caching, DB, pagination tuning
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Performance & Data"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"DB optimizations, caching and pagination performance tuning."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Use indexed queries for common filters (tenant_id, project_id, status)"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Introduce read replicas for heavy reporting queries"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Cache signed assets and heavy lists for 60s; invalidate on write"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"DB migration: validate on staging; keep migrations idempotent and small."}]}
                  ]
                }
                """,
                "Performance & Data:\n- Index queries on tenant_id, project_id, status\n- Use read replicas for heavy reporting\n- Cache heavy lists for 60s; invalidate on write\nDB migration: validate on staging; make migrations idempotent"
            ),
            (
                // Description 7: Security, data protection, compliance, SSO
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Security & Compliance"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Security controls, data protection and SSO integration notes."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Encrypt sensitive data at rest and in transit (TLS 1.2+)" }]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Support SSO via SAML and OIDC; map external groups to roles"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Data retention: purge PII after 90 days by default; provide export endpoints"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Compliance: include GDPR checkbox and data subject request process."}]}
                  ]
                }
                """,
                "Security & Compliance:\n- Encrypt sensitive data at rest and in transit (TLS 1.2+)\n- Support SSO: SAML and OIDC; map groups to roles\n- Data retention: purge PII after 90 days; provide export endpoints\nCompliance: GDPR checkbox and DSR process"
            ),
            (
                // Description 8: Integration, webhooks, examples, retry semantics
                """
                {
                  "type":"doc",
                  "content":[
                    {"type":"heading","attrs":{"level":2},"content":[{"type":"text","text":"Integrations & Webhooks"}]},
                    {"type":"paragraph","content":[{"type":"text","text":"Webhook contract, examples and retry semantics."}]},
                    {"type":"bulletList","content":[
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Emit webhooks for events: user.created, user.updated, invoice.paid"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Webhook retries: 5 attempts with exponential backoff; mark dead after 5 failures"}]}]},
                      {"type":"listItem","content":[{"type":"paragraph","content":[{"type":"text","text":"Provide sample webhook signature verification code for Node and .NET"}]}]}
                    ]},
                    {"type":"paragraph","content":[{"type":"text","text":"Example webhook payload included in docs and Postman collection."}]}
                  ]
                }
                """,
                "Integrations & Webhooks:\n- Emit webhooks for events: user.created, user.updated, invoice.paid\n- Retries: 5 attempts with exponential backoff; mark dead after 5 failures\n- Provide webhook signature verification samples for Node and .NET\nExample: payload included in docs and Postman collection"
            )
        };

        var tasks = new List<Domain.Entities.TaskEntity>();
        var boardColumnIds = context.Message.BoardColumnIds;

        // reuse a single Random to avoid repetition and reseeding issues
        var rnd = new Random();

        logger.LogInformation("Started generating tasks for TenantId: {TenantId}, BoardId: {BoardId}", context.Message.OrganizationId, context.Message.BoardId);
        
        for (var i = 0; i <= 1000; i++)
        {
            var id = Guid.NewGuid();
            var chosen = descriptions[rnd.Next(descriptions.Count)];

            var task = new Domain.Entities.TaskEntity
            {
                Id = id,
                BoardId = context.Message.BoardId,
                BoardColumnId = boardColumnIds[rnd.Next(boardColumnIds.Count)],
                TenantId = context.Message.OrganizationId,
                ProjectId = context.Message.ProjectId,
                Title = $"Task {i} for Board {context.Message.BoardName}",
                Description = chosen.Json,
                DescriptionAsPlainText = chosen.Plain,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = context.Message.CreatedByUserId,
                AssigneeUserId = context.Message.CreatedByUserId,
                TaskTags = tagIds.Select(tid => new Domain.Entities.TaskTagEntity
                {
                    TagId = tid,
                    TaskId = id,
                }).ToList(),
            };

            tasks.Add(task);
        }

        await db.Tasks.AddRangeAsync(tasks);
        await db.SaveChangesAsync();
        
        logger.LogInformation("Finished generating tasks for TenantId: {TenantId}, BoardId: {BoardId}", context.Message.OrganizationId, context.Message.BoardId);
    }
}