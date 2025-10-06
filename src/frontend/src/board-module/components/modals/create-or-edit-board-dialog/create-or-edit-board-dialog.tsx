import type { ModalsPropsBase } from "src/modals-module";
import React from "react";
import { Controller, useFieldArray, useForm } from "react-hook-form";
import {
  Button,
  createListCollection,
  Dialog,
  Field,
  Flex,
  Heading,
  IconButton,
  Input,
  Portal,
  Select,
  Stack,
} from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import type { BoardModel, BoardTypeModel } from "src/board-module/models";
import type { BoardFormValues } from "./create-or-edit-board-dialog.types";
import {
  backlogDefaultBoardColumns,
  kanbanDefaultBoardColumns,
  scrumDefaultBoardColumns,
} from "./create-or-edit-board-dialog.constants.ts";
import { useBoardStore } from "src/board-module/store";

export type CreateOrEditBoardDialogProps = ModalsPropsBase & {
  board?: BoardModel;

  onCreate?: (data: BoardFormValues) => Promise<void>;
  onEdit?: (data: BoardFormValues) => Promise<void>;
};

export const CreateOrEditBoardDialog: React.FC<CreateOrEditBoardDialogProps> = observer(
  ({ board, isOpen, onClose, onCreate, onEdit }) => {
    const boardStore = useBoardStore();

    const { formState, control, register, watch, handleSubmit } = useForm<BoardFormValues>({
      defaultValues: {
        name: board?.name || "",
        typeId: board?.type ? [board.type.id.toString()] : [""],
        columns: board?.columns.map((col) => ({ id: col.id, name: col.name })) ?? [{ name: "" }],
      },
    });
    const { fields, append, remove } = useFieldArray({
      control,
      name: "columns",
    });
    const currentTypeId = watch("typeId");

    const boardTypesById = React.useMemo(() => {
      const map: Record<BoardTypeModel["id"], BoardTypeModel> = {};
      boardStore.boardTypes.forEach((type) => {
        map[type.id] = type;
      });

      return map;
    }, [boardStore.boardTypes]);

    const boardTypesCollection = React.useMemo(
      () =>
        createListCollection({
          items: boardStore.boardTypes.map((type) => ({
            label: type.name.charAt(0).toUpperCase() + type.name.slice(1),
            value: type.id,
          })),
        }),
      [boardStore.boardTypes],
    );

    const defaultReadonlyBoardColumns = React.useMemo(() => {
      if (!currentTypeId[0]) {
        return [];
      }

      switch (boardTypesById[currentTypeId[0]].name.toLowerCase()) {
        case "kanban":
          return kanbanDefaultBoardColumns;
        case "scrum":
          return scrumDefaultBoardColumns;
        case "backlog":
          return backlogDefaultBoardColumns;
        default:
          return [];
      }
    }, [boardTypesById, currentTypeId]);

    const onSubmit = handleSubmit(async (data) => {
      if (board) {
        await onEdit?.(data);
      } else {
        await onCreate?.(data);
      }

      onClose();
    });

    return (
      <Dialog.Root lazyMount placement="center" open={isOpen}>
        <Portal>
          <Dialog.Backdrop />
          <Dialog.Positioner>
            <Dialog.Content>
              <Dialog.Header>
                <Dialog.Title>{board ? "Edit board" : "Create board"}</Dialog.Title>
              </Dialog.Header>
              <Dialog.Body pb="4">
                <Stack gap="6">
                  <Stack gap="2">
                    <Field.Root invalid={!!formState.errors.name}>
                      <Field.Label>Name</Field.Label>
                      <Input
                        {...register("name", {
                          required: {
                            value: true,
                            message: "Name is required",
                          },
                          minLength: {
                            value: 2,
                            message: "Board name must be at least 2 characters long",
                          },
                          pattern: {
                            value: /^(?!.* {2,})[A-Za-z0-9 ]+$/,
                            message:
                              "Board name can only contain letters, numbers, and spaces, and cannot contain consecutive spaces",
                          },
                        })}
                        placeholder="Enter board name"
                      />
                      <Field.ErrorText>{formState.errors.name?.message}</Field.ErrorText>
                    </Field.Root>

                    <Field.Root invalid={!!formState.errors.typeId}>
                      <Field.Label>Board type</Field.Label>
                      <Controller
                        control={control}
                        name="typeId"
                        render={({ field }) => (
                          <Select.Root
                            name={field.name}
                            value={field.value}
                            onValueChange={(item) => field.onChange(item.value)}
                            onInteractOutside={() => field.onBlur()}
                            collection={boardTypesCollection}
                          >
                            <Select.HiddenSelect />
                            <Select.Control>
                              <Select.Trigger>
                                <Select.ValueText placeholder="Select role" />
                              </Select.Trigger>
                              <Select.IndicatorGroup>
                                <Select.Indicator />
                              </Select.IndicatorGroup>
                            </Select.Control>
                            <Select.Positioner>
                              <Select.Content>
                                {boardTypesCollection.items.map((item) => (
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
                      <Field.ErrorText>{formState.errors.typeId?.message}</Field.ErrorText>
                    </Field.Root>
                  </Stack>

                  {!board && defaultReadonlyBoardColumns.length > 0 ? (
                    <Stack gap="2">
                      <Heading size="sm">Default columns/statuses</Heading>

                      {defaultReadonlyBoardColumns.map((column, index) => (
                        <Input value={column} key={index} readOnly={true} disabled={true} />
                      ))}
                    </Stack>
                  ) : null}

                  <Stack gap="2">
                    <Heading size="sm">
                      {defaultReadonlyBoardColumns.length > 0
                        ? "Additional columns/statuses"
                        : "Columns/statuses"}
                    </Heading>

                    {fields.map((field, index) => (
                      <Flex gap="4" key={field.id}>
                        <Field.Root invalid={!!formState.errors.columns?.[index]?.name}>
                          <Input
                            {...register(`columns.${index}.name`, {
                              required: {
                                value: true,
                                message: "Column name is required",
                              },
                              minLength: {
                                value: 2,
                                message: "Column name must be at least 2 characters long",
                              },
                              pattern: {
                                value: /^(?!.* {2,})[A-Za-z0-9 ]+$/,
                                message:
                                  "Column name can only contain letters, numbers, and spaces, and cannot contain consecutive spaces",
                              },
                              validate: (value) => {
                                if (!board && defaultReadonlyBoardColumns.includes(value)) {
                                  return "This column name is reserved and cannot be used";
                                }

                                return true;
                              },
                            })}
                            placeholder="Enter column/status name"
                          />
                          <Field.ErrorText>
                            {formState.errors.columns?.[index]?.name?.message}
                          </Field.ErrorText>
                        </Field.Root>

                        <IconButton
                          variant="outline"
                          colorPalette="red"
                          onClick={() => remove(index)}
                        >
                          -
                        </IconButton>
                      </Flex>
                    ))}

                    <Button variant="ghost" onClick={() => append({ name: "" })}>
                      + Add board column
                    </Button>
                  </Stack>
                </Stack>
              </Dialog.Body>
              <Dialog.Footer>
                <Dialog.ActionTrigger asChild>
                  <Button variant="outline" onClick={onClose}>
                    Cancel
                  </Button>
                </Dialog.ActionTrigger>
                <Button
                  loading={formState.isSubmitting}
                  onClick={onSubmit}
                  disabled={!formState.isDirty}
                >
                  Save
                </Button>
              </Dialog.Footer>
            </Dialog.Content>
          </Dialog.Positioner>
        </Portal>
      </Dialog.Root>
    );
  },
);
