import React from "react";

import { Dialog, Button, Text } from "@chakra-ui/react";

import type { ModalsPropsBase } from "src/modals-module";

export interface IConfirmModalProps extends ModalsPropsBase {
  title: string;
  description?: string;
  onConfirm: () => void | Promise<unknown>;
}

export const ConfirmModal: React.FC<IConfirmModalProps> = ({
  title,
  description,
  onConfirm,
  onClose,
  isOpen,
}) => {
  const [isLoading, setIsLoading] = React.useState(false);
  const handleConfirm = async () => {
    setIsLoading(true);

    try {
      await onConfirm();
      setIsLoading(false);
    } finally {
      onClose();
    }
  };

  return (
    <Dialog.Root lazyMount placement="center" open={isOpen}>
      <Dialog.Backdrop />
      <Dialog.Positioner>
        <Dialog.Content>
          <Dialog.Header justifyContent="center">
            <Dialog.Title>{title}</Dialog.Title>
          </Dialog.Header>
          {description ? (
            <Dialog.Body display="flex" justifyContent="center">
              <Text>{description}</Text>
            </Dialog.Body>
          ) : null}
          <Dialog.Footer>
            <Button onClick={onClose}>Cancel</Button>
            <Button variant="outline" onClick={handleConfirm} loading={isLoading}>
              Confirm
            </Button>
          </Dialog.Footer>
        </Dialog.Content>
      </Dialog.Positioner>
    </Dialog.Root>
  );
};
