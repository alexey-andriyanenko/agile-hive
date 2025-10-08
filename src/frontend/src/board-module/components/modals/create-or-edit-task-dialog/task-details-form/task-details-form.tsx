import React from "react";
import {
  Avatar,
  Box,
  createListCollection,
  Field,
  Flex,
  Heading,
  Input,
  Select,
  Stack,
} from "@chakra-ui/react";
import { useFormContext, Controller } from "react-hook-form";

import { TipTapEditor } from "src/board-module/components/tiptap-editor";
import type { BoardModel } from "src/board-module/models";
import { projectUserApiService } from "src/project-module/api";
import { useOrganizationStore } from "src/organization-module/store";
import type { TaskFormValues } from "../create-or-edit-task-dialog.types.ts";
import type { ProjectUserModel } from "src/project-module/models";
import { pickColor } from "src/shared-module/utils";
import { useTagStore } from "src/project-module/store";

type TaskDetailsFormProps = {
  board: BoardModel;
};

export const TaskDetailsForm: React.FC<TaskDetailsFormProps> = ({ board }) => {
  const organizationStore = useOrganizationStore();
  const tagStore = useTagStore();

  const { formState, control, register } = useFormContext<TaskFormValues>();

  const [assignees, setAssignees] = React.useState<ProjectUserModel[]>([]);

  React.useEffect(() => {
    projectUserApiService
      .getManyProjectUsers({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: board.projectId,
      })
      .then((res) => {
        setAssignees(res.users);
      });

    if (tagStore.tags.length === 0) {
      tagStore.fetchTagsByProjectId(
        organizationStore.currentOrganization!.id,
        board.projectId
      );
    }
  }, []);

  const boardColumnsCollection = React.useMemo(() => {
    return createListCollection({
      items: board.columns.map((column) => ({ label: column.name, value: column.id })),
    });
  }, [board.columns]);

  const assigneesCollection = React.useMemo(() => {
    return createListCollection({
      items: assignees.map((user) => ({
        label: `${user.firstName} ${user.lastName}`,
        value: user.id,
      })),
    });
  }, [assignees]);

  const tagsCollection = React.useMemo(() => {
    return createListCollection({
      items: tagStore.tags.map((tag) => ({
        label: tag.name,
        value: tag.id,
        color: tag.color,
      })),
    });
  }, [tagStore.tags]);

  return (
    <Stack gap="4">
      <Heading size="sm">Task details</Heading>
      <Stack gap="2">
        <Field.Root invalid={!!formState.errors.title}>
          <Field.Label>Title</Field.Label>
          <Input
            {...register("title", {
              required: { value: true, message: "Title is required" },
            })}
          />
          <Field.ErrorText>{formState.errors.title?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.boardColumnId}>
          <Field.Label>Status</Field.Label>
          <Controller
            control={control}
            name="boardColumnId"
            rules={{
              required: { value: true, message: "Status is required" },
            }}
            render={({ field }) => (
              <Select.Root
                name={field.name}
                value={field.value}
                onValueChange={(item) => field.onChange(item.value)}
                onInteractOutside={() => field.onBlur()}
                collection={boardColumnsCollection}
              >
                <Select.HiddenSelect />
                <Select.Control>
                  <Select.Trigger>
                    <Select.ValueText placeholder="Select status" />
                  </Select.Trigger>
                  <Select.IndicatorGroup>
                    <Select.Indicator />
                  </Select.IndicatorGroup>
                </Select.Control>
                <Select.Positioner>
                  <Select.Content>
                    {boardColumnsCollection.items.map((item) => (
                      <Select.Item key={item.value} item={item}>
                        {item.label}
                        <Select.ItemIndicator />
                      </Select.Item>
                    ))}
                  </Select.Content>
                </Select.Positioner>
              </Select.Root>
            )}
          />
          <Field.ErrorText>{formState.errors.boardColumnId?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.assigneeId}>
          <Field.Label>Assignee</Field.Label>
          <Controller
            control={control}
            name="assigneeId"
            rules={{
              required: { value: true, message: "Assignee is required" },
            }}
            render={({ field }) => (
              <Select.Root
                name={field.name}
                value={field.value}
                onValueChange={(item) => field.onChange(item.value)}
                onInteractOutside={() => field.onBlur()}
                collection={assigneesCollection}
              >
                <Select.HiddenSelect />
                <Select.Control>
                  <Select.Trigger>
                    <Select.ValueText placeholder="Select assignee" />
                  </Select.Trigger>
                  <Select.IndicatorGroup>
                    <Select.Indicator />
                  </Select.IndicatorGroup>
                </Select.Control>
                <Select.Positioner>
                  <Select.Content>
                    {assigneesCollection.items.map((item) => (
                      <Select.Item key={item.value} item={item}>
                        <Flex alignItems="center" gap={4}>
                          <Avatar.Root colorPalette={pickColor(item.label)} size="sm">
                            <Avatar.Fallback name={item.label} />
                          </Avatar.Root>

                          {item.label}
                        </Flex>

                        <Select.ItemIndicator />
                      </Select.Item>
                    ))}
                  </Select.Content>
                </Select.Positioner>
              </Select.Root>
            )}
          />
          <Field.ErrorText>{formState.errors.assigneeId?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.tagIds}>
          <Field.Label>Tags</Field.Label>
          <Controller
            control={control}
            name="tagIds"
            render={({ field }) => (
              <Select.Root
                name={field.name}
                value={field.value}
                onValueChange={(item) => field.onChange(item.value)}
                onInteractOutside={() => field.onBlur()}
                collection={tagsCollection}
                multiple
              >
                <Select.HiddenSelect />
                <Select.Control>
                  <Select.Trigger>
                    <Select.ValueText placeholder="Select tags" />
                  </Select.Trigger>
                  <Select.IndicatorGroup>
                    <Select.Indicator />
                  </Select.IndicatorGroup>
                </Select.Control>
                <Select.Positioner>
                  <Select.Content>
                    {tagsCollection.items.map((item) => (
                      <Select.Item key={item.value} item={item}>
                        <Flex alignItems="center" gap={4}>
                          {item.label}
                          <Box width="40px" height="20px" bgColor={item.color} borderRadius="8px" />
                        </Flex>

                        <Select.ItemIndicator />
                      </Select.Item>
                    ))}
                  </Select.Content>
                </Select.Positioner>
              </Select.Root>
            )}
          />
          <Field.ErrorText>{formState.errors.tagIds?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.description}>
          <Field.Label>Description</Field.Label>
          <Controller
            name="description"
            control={control}
            rules={{
              required: { value: true, message: "Description is required" },
            }}
            render={({ field }) => <TipTapEditor value={field.value} onChange={field.onChange} />}
          />
          <Field.ErrorText>{formState.errors.description?.message}</Field.ErrorText>
        </Field.Root>
      </Stack>
    </Stack>
  );
};
