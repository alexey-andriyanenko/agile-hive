using BoardMessages.Messages;
using MassTransit;
using TaskService.Infrastructure.Data;

namespace TaskService.Application.Consumers;

public class BoardMessagesConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<BoardCreatedMessage>
{
    public async Task Consume(ConsumeContext<BoardCreatedMessage> context)
    {
        var tagIds = new List<Guid>()
        {
            Guid.Parse("0a1b2c3d-4e5f-4a6b-8c7d-1e2f3a4b5c6d"),
            Guid.Parse("2c3d4e5f-6a7b-4c8d-8e9f-3a4b5c6d7e8f"),
            Guid.Parse("3d4e5f6a-7b8c-4d9e-8f0a-4b5c6d7e8f90"),
            Guid.Parse("4e5f6a7b-8c9d-4e0f-8a1b-5c6d7e8f9012"),
            Guid.Parse("8c9d0e1f-2a3b-4c4d-8e5f-90123456789a"),
            Guid.Parse("5f6a7b8c-9d0e-4f1a-8b2c-6d7e8f901234"),
        };
        
        var tasks = new List<Domain.Entities.TaskEntity>();
        var boardColumnIds = context.Message.BoardColumnIds;
        
        for(var i = 0; i <= 10000; i++)
        {
            var id = Guid.NewGuid();
            var task = new Domain.Entities.TaskEntity
            {
                Id = id,
                BoardId = context.Message.BoardId,
                BoardColumnId = boardColumnIds[new Random().Next(boardColumnIds.Count)],
                TenantId = context.Message.OrganizationId,
                ProjectId = context.Message.ProjectId,
                Title = $"Task {i} for Board {context.Message.BoardName}",
                Description = """
                              {
                                  "type": "doc",
                                  "content": [
                                      {
                                          "type": "paragraph",
                                          "content": [
                                              {
                                                  "type": "text",
                                                  "text": "Acceptance criteria:"
                                              }
                                          ]
                                      },
                                      {
                                          "type": "bulletList",
                                          "content": [
                                              {
                                                  "type": "listItem",
                                                  "content": [
                                                      {
                                                          "type": "paragraph",
                                                          "content": [
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "bold"
                                                                      }
                                                                  ],
                                                                  "text": "/register "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": "should accept "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "firstname,"
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": " "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "lastname, username "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": "and "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "password"
                                                              }
                                                          ]
                                                      }
                                                  ]
                                              },
                                              {
                                                  "type": "listItem",
                                                  "content": [
                                                      {
                                                          "type": "paragraph",
                                                          "content": [
                                                              {
                                                                  "type": "text",
                                                                  "text": "after succesful "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "register "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": "user should receive "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "link",
                                                                          "attrs": {
                                                                              "href": "https://google.com",
                                                                              "target": "_blank",
                                                                              "rel": "noopener noreferrer nofollow",
                                                                              "class": null
                                                                          }
                                                                      }
                                                                  ],
                                                                  "text": "invite link"
                                                              }
                                                          ]
                                                      }
                                                  ]
                                              },
                                              {
                                                  "type": "listItem",
                                                  "content": [
                                                      {
                                                          "type": "paragraph",
                                                          "content": [
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "bold"
                                                                      }
                                                                  ],
                                                                  "text": "/login "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": "should accept "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "username "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": "and "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "italic"
                                                                      }
                                                                  ],
                                                                  "text": "password"
                                                              }
                                                          ]
                                                      }
                                                  ]
                                              },
                                              {
                                                  "type": "listItem",
                                                  "content": [
                                                      {
                                                          "type": "paragraph",
                                                          "content": [
                                                              {
                                                                  "type": "text",
                                                                  "text": "after succesful "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "bold"
                                                                      }
                                                                  ],
                                                                  "text": "login"
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "text": " user should be redirected to "
                                                              },
                                                              {
                                                                  "type": "text",
                                                                  "marks": [
                                                                      {
                                                                          "type": "link",
                                                                          "attrs": {
                                                                              "href": "https://google.com",
                                                                              "target": "_blank",
                                                                              "rel": "noopener noreferrer nofollow",
                                                                              "class": null
                                                                          }
                                                                      }
                                                                  ],
                                                                  "text": "home page"
                                                              }
                                                          ]
                                                      }
                                                  ]
                                              }
                                          ]
                                      },
                                      {
                                          "type": "paragraph"
                                      }
                                  ]
                              }
                              """,
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
        
        await dbContext.Tasks.AddRangeAsync(tasks);
        await dbContext.SaveChangesAsync();
    }
}