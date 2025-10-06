import React from "react";

import { Controller, useFieldArray, useFormContext } from "react-hook-form";
import type { TaskFormValues } from "../create-or-edit-task-dialog.types.ts";
import { Button, Field, Stack } from "@chakra-ui/react";
import { CommentEditor } from "src/board-module/components/comment-editor";

export const TaskCommentsForm: React.FC = () => {
  const { formState, control } = useFormContext<TaskFormValues>();
  const { fields, append, remove } = useFieldArray({
    control,
    name: "comments",
  });

  return (
    <Stack gap="2">
      {fields.map((field, index) => (
        <Field.Root invalid={!!formState.errors.comments?.[index]?.content} key={field.id}>
          <Controller
            name={`comments.${index}.content`}
            control={control}
            rules={{
              required: { value: true, message: "Content is required" },
            }}
            render={({ field }) => (
              <CommentEditor
                value={field.value}
                onChange={field.onChange}
                onDelete={() => remove(index)}
              />
            )}
          />
          <Field.ErrorText>{formState.errors.comments?.[index]?.content?.message}</Field.ErrorText>
        </Field.Root>
      ))}

      <Button variant="ghost" onClick={() => append({ content: "" })} size="sm">
        + Add Comment
      </Button>
    </Stack>
  );
};
